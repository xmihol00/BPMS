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

        public async Task<ServiceDetailPartialDTO> Edit(Guid id)
        {
            return await EditPageDTO(id, true);
        }

        public async Task<ServiceDetailPartialDTO> DetailPartial(Guid id)
        {
            return await EditPageDTO(id, false);
        }

        public async Task<ServiceDetailDTO> Detail(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new ServiceDetailDTO();
            }
            else
            {
                return await EditPageDTO(id, true);
            }
        }

        private async Task<ServiceDetailDTO> EditPageDTO(Guid id, bool otherSystems = false)
        {
            ServiceDetailDTO dto = await _serviceRepository.Edit(id);
            dto.InputAttributes = ServiceTreeHelper.CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Input));
            dto.OutputAttributes = ServiceTreeHelper.CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Output));
            dto.Headers = await _serviceHeaderRepository.All(id);
            if (otherSystems)
            {
                dto.OtherServices = await _serviceRepository.All(id);
                dto.SelectedService = await _serviceRepository.Selected(id);
            }
            
            return dto;
        }

        public async Task<ServiceDetailDTO> EditPartial(Guid id)
        {
            ServiceDetailDTO dto = await _serviceRepository.Edit(id);
            dto.InputAttributes = ServiceTreeHelper.CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Input));
            dto.OutputAttributes = ServiceTreeHelper.CreateTree(await _serviceDataSchemaRepository.DataSchemas(id, DirectionEnum.Output));
            dto.Headers = await _serviceHeaderRepository.All(id);
            return dto;
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

        public async Task<IEnumerable<IDataSchemaNode>> GenerateAttributes(ServiceCallResultDTO dto)
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

            return ServiceTreeHelper.CreateTree(await _serviceDataSchemaRepository.DataSchemas(dto.ServiceId, DirectionEnum.Output));
        }

        public async Task<ServiceCallResultDTO> SendRequest(IFormCollection data)
        {
            ServiceRequestDTO service = await _serviceRepository.ForRequest(Guid.Parse(data["ServiceId"].First()));
            service.Nodes = await CreateRequestTree(service.Id, data);
            ServiceCallResultDTO result = await new WebServiceHelper(service).SendRequest();
            FormatResult(result);
            result.ServiceId = service.Id;
            
            return result;
        }

        private void FormatResult(ServiceCallResultDTO result)
        {
            if (result.Serialization == null)
            {
                try
                {
                    result.RecievedData = JObject.Parse("{\"data\":" + result.RecievedData + "}").ToString(Formatting.Indented);
                }
                catch 
                {

                }
            }
            else if (result.Serialization == SerializationEnum.JSON)
            {
                try
                {
                    result.RecievedData = JObject.Parse(result.RecievedData).ToString(Formatting.Indented);
                }
                catch
                {

                }
            }
            else if (result.Serialization == SerializationEnum.XML)
            {
                // TODO
            }
        }

        public async Task<string> GenerateRequest(IFormCollection data)
        {
            ServiceRequestDTO service = await _serviceRepository.ForRequest(Guid.Parse(data["ServiceId"].First()));
            service.Nodes = await CreateRequestTree(service.Id, data);

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

        public async Task<Guid> Create(ServiceCreateEditDTO dto)
        {
            ServiceEntity entity = _mapper.Map<ServiceEntity>(dto);
            await _serviceRepository.Create(entity);
            await _serviceRepository.Save();

            return entity.Id;
        }

        public async Task<ServiceInfoCardDTO> Edit(ServiceCreateEditDTO dto)
        {
            ServiceEntity entity = _mapper.Map<ServiceEntity>(dto);
            _serviceRepository.Update(entity);
            await _serviceRepository.Save();
            
            ServiceInfoCardDTO card = _mapper.Map<ServiceInfoCardDTO>(entity);
            card.SelectedService = await _serviceRepository.Selected(entity.Id);
            return card;
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

            return ServiceTreeHelper.CreateTree(await _serviceDataSchemaRepository.DataSchemas(dto.ServiceId, dto.Direction));
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
            IEnumerable<DataSchemaDataDTO> nodes = await _serviceDataSchemaRepository.DataSchemaToSend(serviceId);
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

            return ServiceTreeHelper.CreateTree<DataSchemaDataDTO>(nodes);
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

                    case JTokenType.Boolean:
                        await CreateOutputDataSchema(name, DataTypeEnum.Bool, parentId);
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
