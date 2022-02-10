using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.User;

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

        private IEnumerable<ServiceDataSchemaNodeDTO> CreateTree(IEnumerable<ServiceDataSchemaNodeDTO> allNodes, Guid? parentId)
        {
            IEnumerable<ServiceDataSchemaNodeDTO> nodes = allNodes.Where(x => x.ParentId == parentId);
            foreach (ServiceDataSchemaNodeDTO node in nodes)
            {
                if (node.Type == DataTypeEnum.Object)
                {
                    node.Children = CreateTree(allNodes, node.Id);
                }
            }

            return nodes;
        }
    }
}
