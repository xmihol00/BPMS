using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BPMS_BL.Facades
{
    public class ServiceFacade
    {
        private readonly ServiceDataSchemaRepository _serviceDataSchemaRepository;
        private readonly ServiceRepository _serviceRepository;

        private readonly IMapper _mapper;

        public ServiceFacade(ServiceDataSchemaRepository serviceDataSchemaRepository, ServiceRepository serviceRepository, 
                             IMapper mapper)
        {
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<ServiceEditPageDTO> CreateEdit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new ServiceEditPageDTO();
            }
            else
            {
                ServiceEditPageDTO dto = await _serviceRepository.Edit(id);
                dto.InputAttributes = CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Input), null);
                dto.OutputAttributes = CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Output), null);
                return dto;
            }
        }

        public async Task RemoveSchema(Guid id)
        {
            List<Guid?> removedIds = new List<Guid?> () { id };
            foreach (ServiceDataSchemaEntity schema in await _serviceDataSchemaRepository.SchemasForRemoval(id))
            {
                if (removedIds.Contains(schema.ParentId))
                {
                    removedIds.Add(schema.ParentId);
                    _serviceDataSchemaRepository.Remove(schema);
                }
            }
            _serviceDataSchemaRepository.Remove(new ServiceDataSchemaEntity() { Id = id });

            await _serviceDataSchemaRepository.Save();
        }

        public async Task<string> SendRequest(IFormCollection data)
        {
            ServiceEntity service = await _serviceRepository.Detail(Guid.Parse(data["ServiceId"].First()));
            IEnumerable<ServiceDataSchemaDataDTO> nodes = await CreateRequestTree(service.Id, data);
            Uri url = new Uri(service.URL);

            return await new HttpRequestHelper(nodes, service.Serialization, url, service.HttpMethod).SendRequest();
        }

        public async Task<string> GenerateRequest(IFormCollection data)
        {
            ServiceEntity service = await _serviceRepository.Detail(Guid.Parse(data["ServiceId"].First()));
            IEnumerable<ServiceDataSchemaDataDTO> nodes = await CreateRequestTree(service.Id, data);
            Uri url = new Uri(service.URL);

            return new HttpRequestHelper(nodes, service.Serialization, url, service.HttpMethod).GenerateRequest();
        }

        public async Task<ServiceDataSchemaTestDTO> Test(Guid id)
        {
            return new ServiceDataSchemaTestDTO()
            {
                ServiceId = id,
                Schemas = await _serviceDataSchemaRepository.Test(id)
            };
        }

        public async Task<Guid> CreateEdit(ServiceCreateEditDTO dto)
        {
            ServiceEntity entity = _mapper.Map<ServiceEntity>(dto);
            if (dto.Id == Guid.Empty)
            {
                await _serviceRepository.Create(entity);
            }
            else
            {
                _serviceRepository.Update(entity);
            }

            await _serviceRepository.Save();
            return entity.Id;
        }

        public async Task<ServiceOverviewDTO> Overview()
        {
            return new ServiceOverviewDTO()
            {
                Services = await _serviceRepository.All()
            };
        }

        public async Task<IEnumerable<ServiceDataSchemaNodeDTO>> CreateEditSchema(ServiceDataSchemaCreateEditDTO dto)
        {
            ServiceDataSchemaEntity entity = _mapper.Map<ServiceDataSchemaEntity>(dto);
            
            if (dto.Id == Guid.Empty)
            {
                await _serviceDataSchemaRepository.Create(entity);
            }
            else
            {
                _serviceDataSchemaRepository.Update(entity);
            }

            await _serviceDataSchemaRepository.Save();

            return CreateTree(await _serviceDataSchemaRepository.DataSchemas(dto.ServiceId, DirectionEnum.Input), null);
        }

        private IEnumerable<T> CreateTree<T>(IEnumerable<T> allNodes, Guid? parentId = null) where T : ServiceDataSchemaNode
        {
            IEnumerable<T> nodes = allNodes.Where(x => x.ParentId == parentId);
            foreach (T node in nodes)
            {
                if (node.Type == DataTypeEnum.Object)
                {
                    node.Children = CreateTree(allNodes, node.Id);
                }
            }

            return nodes;
        }

        private async Task<IEnumerable<ServiceDataSchemaDataDTO>> CreateRequestTree(Guid serviceId, IFormCollection data)
        {
            IEnumerable<ServiceDataSchemaDataDTO> nodes = CreateTree(await _serviceDataSchemaRepository.DataSchemasTest(serviceId));
            foreach (ServiceDataSchemaDataDTO node in nodes)
            {
                if (node.StaticData == null)
                {
                    node.Data = data[$"{node.Id}"].FirstOrDefault();    
                }
                else
                {
                    node.Data = node.StaticData;
                }
            }

            return nodes;
        }
    }
}
