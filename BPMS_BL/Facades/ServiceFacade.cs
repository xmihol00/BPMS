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
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMS_BL.Facades
{
    public class ServiceFacade
    {
        private readonly ServiceDataSchemaRepository _serviceDataSchemaRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly ServiceHeaderRepository _serviceHeaderRepository;
        private Guid _serviceId;

        private readonly IMapper _mapper;

        public ServiceFacade(ServiceDataSchemaRepository serviceDataSchemaRepository, ServiceRepository serviceRepository, 
                             ServiceHeaderRepository serviceHeaderRepository, IMapper mapper)
        {
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
            _serviceRepository = serviceRepository;
            _serviceHeaderRepository = serviceHeaderRepository;
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
                dto.InputAttributes = CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Input));
                dto.OutputAttributes = CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Output));
                dto.Headers = await _serviceHeaderRepository.All(id);
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

        public async Task RemoveHeader(Guid id)
        {
            _serviceHeaderRepository.Remove(new ServiceHeaderEntity() { Id = id });
            await _serviceHeaderRepository.Save();
        }

        public async Task<List<HeaderAllDTO>> CreateEditHeader(HeaderCreateEditDTO dto)
        {
            ServiceHeaderEntity entity = _mapper.Map<ServiceHeaderEntity>(dto);

            if (dto.Id == Guid.Empty)
            {
                await _serviceHeaderRepository.Create(entity);
            }
            else
            {
                _serviceHeaderRepository.Update(entity);
            }
            await _serviceHeaderRepository.Save();

            return await _serviceHeaderRepository.All(dto.ServiceId);
        }

        public async Task<IEnumerable<IDataSchemaNode>> GenerateAttributes(ServiceTestResultDTO dto)
        {
            _serviceId = dto.ServiceId;

            IDbContextTransaction transaction = await _serviceDataSchemaRepository.CreateTransaction();
            
            if (dto.Serialization == SerializationEnum.JSON)
            {
                await ParseJObject(JObject.Parse(dto.RecievedData));
            }
            else
            {
                throw new NotImplementedException();
            }
            await transaction.CommitAsync();

            return CreateTree(await _serviceDataSchemaRepository.DataSchemas(dto.ServiceId, DirectionEnum.Output));
        }

        public async Task<ServiceTestResultDTO> SendRequest(IFormCollection data)
        {
            ServiceRequestDTO service = await _serviceRepository.ForRequest(Guid.Parse(data["ServiceId"].First()));
            service.Nodes = await CreateRequestTree(service.Id, data);
            service.Headers = await _serviceHeaderRepository.ForRequest(service.Id);
            ServiceTestResultDTO result = await new WebServiceHelper(service).SendRequest();
            result.ServiceId = service.Id;
            
            return result;
        }

        public async Task<string> GenerateRequest(IFormCollection data)
        {
            ServiceRequestDTO service = await _serviceRepository.ForRequest(Guid.Parse(data["ServiceId"].First()));
            service.Nodes = await CreateRequestTree(service.Id, data);
            service.Headers = await _serviceHeaderRepository.ForRequest(service.Id);

            return new WebServiceHelper(service).GenerateRequest();
        }

        public async Task<DataSchemaTestDTO> Test(Guid id)
        {
            return new DataSchemaTestDTO()
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

        public async Task<IEnumerable<DataSchemaNodeDTO>> CreateEditSchema(DataSchemaCreateEditDTO dto)
        {
            ServiceDataSchemaEntity entity = _mapper.Map<ServiceDataSchemaEntity>(dto);
            if (entity.Direction == DirectionEnum.Output)
            {
                entity.Compulsory = true;
            }
            
            if (dto.Id == Guid.Empty)
            {
                await _serviceDataSchemaRepository.Create(entity);
            }
            else
            {
                _serviceDataSchemaRepository.Update(entity);
            }

            await _serviceDataSchemaRepository.Save();

            return CreateTree(await _serviceDataSchemaRepository.DataSchemas(dto.ServiceId, dto.Direction));
        }

        private IEnumerable<T> CreateTree<T>(IEnumerable<T> allNodes, Guid? parentId = null) where T : DataSchema
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

        private async Task<Guid> CreateOutputDataSchema(string name, DataTypeEnum type, Guid? parentId)
        {
            ServiceDataSchemaEntity? current = await _serviceDataSchemaRepository.Find(_serviceId, name, parentId, DirectionEnum.Output);
            
            if (current != null)
            {
                return current.Id;
            }
            
            ServiceDataSchemaEntity dataSchema = new ServiceDataSchemaEntity()
            {
                Alias = name,
                Name = name,
                Compulsory = true,
                Direction = DirectionEnum.Output,
                ParentId = parentId,
                ServiceId = _serviceId,
                Type = type,
                StaticData = null
            };

            await _serviceDataSchemaRepository.Create(dataSchema);
            await _serviceDataSchemaRepository.Save();
            return dataSchema.Id;
        }

        private async Task<IEnumerable<DataSchemaDataDTO>> CreateRequestTree(Guid serviceId, IFormCollection data)
        {
            IEnumerable<DataSchemaDataDTO> nodes = await _serviceDataSchemaRepository.DataSchemasTest(serviceId);
            foreach (DataSchemaDataDTO node in nodes)
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

            nodes = CreateTree<DataSchemaDataDTO>(nodes);

            return nodes;
        }

        private async Task ParseJObject(IEnumerable<JToken> tokens, Guid? parentId = null)
        {
            foreach (JProperty property in tokens)
            {
                string name = property.Name;
                switch (property.Value.Type)
                {
                    case JTokenType.Object:
                        await ParseJObject(property.Value.Children(), await CreateOutputDataSchema(name, DataTypeEnum.Object, parentId));
                        break;
                    
                    case JTokenType.String:
                        await CreateOutputDataSchema(name, DataTypeEnum.String, parentId);
                        break;
                    
                    case JTokenType.Float:
                    case JTokenType.Integer:
                        await CreateOutputDataSchema(name, DataTypeEnum.Number, parentId);
                        break;
                    
                    case JTokenType.Array:
                        // TODO
                        //await ParseJObject(property.Descendants(), await CreateOutputDataSchema(name, DataTypeEnum.Array, parentId));
                        break;
                }
            }
        }
    }
}
