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
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BPMS_DTOs.Filter;

namespace BPMS_BL.Facades
{
    public class ServiceFacade : BaseFacade
    {
        private readonly DataSchemaRepository _dataSchemaRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly ServiceHeaderRepository _serviceHeaderRepository;
        private Guid _serviceId;

        private readonly IMapper _mapper;

        public ServiceFacade(DataSchemaRepository dataSchemaRepository, ServiceRepository serviceRepository, 
                             ServiceHeaderRepository serviceHeaderRepository, FilterRepository filterRepository, IMapper mapper)
        : base(filterRepository)
        {
            _dataSchemaRepository = dataSchemaRepository;
            _serviceRepository = serviceRepository;
            _serviceHeaderRepository = serviceHeaderRepository;
            _mapper = mapper;
        }

        public void SetFilters(bool[] filters, Guid userId)
        {
            _serviceRepository.Filters = filters;
            _serviceRepository.UserId = userId;
            _userId = userId;
        }

        public async Task<List<ServiceAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, _userId);
            _serviceRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _serviceRepository.All();
        }

        public async Task<ServiceDetailDTO> DetailPartial(Guid id)
        {
            ServiceDetailDTO dto = await _serviceRepository.Detail(id);
            dto.InputAttributes = ServiceTreeHelper.CreateTree(await _dataSchemaRepository.DataSchemas(id, DirectionEnum.Input));
            dto.OutputAttributes = ServiceTreeHelper.CreateTree(await _dataSchemaRepository.DataSchemas(id, DirectionEnum.Output));

            return dto;
        }

        public async Task<ServiceDetailDTO> Detail(Guid id)
        {
            ServiceDetailDTO dto = await DetailPartial(id);
            dto.OtherServices = await _serviceRepository.All(id);
            dto.SelectedService = await _serviceRepository.Selected(id);

            return dto;
        }

        public async Task RemoveSchema(Guid id)
        {
            List<Guid?> removedIds = new List<Guid?> () { id };
            foreach (DataSchemaEntity schema in await _dataSchemaRepository.SchemasForRemoval(id))
            {
                if (removedIds.Contains(schema.ParentId))
                {
                    removedIds.Add(schema.ParentId);
                    _dataSchemaRepository.Remove(schema);
                }
            }
            _dataSchemaRepository.Remove(new DataSchemaEntity() { Id = id });

            await _dataSchemaRepository.Save();
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

            IDbContextTransaction transaction = await _dataSchemaRepository.CreateTransaction();
            
            if (dto.Serialization == SerializationEnum.JSON)
            {
                await ParseJObject(JObject.Parse(dto.RecievedData));
            }
            else
            {
                throw new NotImplementedException();
            }
            await transaction.CommitAsync();

            return ServiceTreeHelper.CreateTree(await _dataSchemaRepository.DataSchemas(dto.ServiceId, DirectionEnum.Output));
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
                Schemas = await _dataSchemaRepository.Test(id)
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
            ServiceEntity entity = await _serviceRepository.Bare(dto.Id);
            if (dto.AuthType == ServiceAuthEnum.None)
            {
                dto.AppId = null;
                dto.AppSecret = null;
            }
            else
            {
                if (dto.AppSecret == null)
                {
                    dto.AppSecret = entity.AppSecret;
                }
            }
            _mapper.Map<ServiceCreateEditDTO, ServiceEntity>(dto, entity);
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
            DataSchemaEntity entity = _mapper.Map<DataSchemaEntity>(dto);
            if (entity.Direction == DirectionEnum.Output)
            {
                entity.Compulsory = true;
            }
            
            if (dto.Id == Guid.Empty)
            {
                await _dataSchemaRepository.Create(entity);
            }
            else
            {
                _dataSchemaRepository.Update(entity);
            }

            await _dataSchemaRepository.Save();

            return ServiceTreeHelper.CreateTree(await _dataSchemaRepository.DataSchemas(dto.ServiceId, dto.Direction));
        }

        private async Task<Guid> CreateOutputDataSchema(string? name, DataTypeEnum type, Guid? parentId, bool array)
        {
            DataSchemaEntity? current = await _dataSchemaRepository.Find(_serviceId, name, parentId, DirectionEnum.Output);
            
            if (current != null)
            {
                return current.Id;
            }
            
            DataSchemaEntity dataSchema = new DataSchemaEntity()
            {
                Alias = name,
                Name = name,
                Compulsory = true,
                Direction = DirectionEnum.Output,
                ParentId = parentId,
                ServiceId = _serviceId,
                Type = type,
                StaticData = null,
                Array = array
            };

            await _dataSchemaRepository.Create(dataSchema);
            await _dataSchemaRepository.Save();
            return dataSchema.Id;
        }

        private async Task<IEnumerable<DataSchemaDataDTO>> CreateRequestTree(Guid serviceId, IFormCollection data)
        {
            List<DataSchemaDataDTO> arrayNodes = new List<DataSchemaDataDTO>();
            List<DataSchemaDataDTO> nodes = await _dataSchemaRepository.DataSchemaToSend(serviceId);
            foreach (DataSchemaDataDTO node in nodes)
            {
                if (node.Type > DataTypeEnum.Object)
                {
                    string value = null;
                    int i = 0;
                    while ((value = data[$"{node.Id}_{++i}"].FirstOrDefault()) != null)
                    {
                        arrayNodes.Add(new DataSchemaDataDTO
                        {
                            ParentId = node.Id,
                            Data = value,
                            Type = node.Type - 4
                        });
                    }
                }
                else
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
            }

            nodes.AddRange(arrayNodes);
            return ServiceTreeHelper.CreateTree<DataSchemaDataDTO>(nodes);
        }

        private async Task ParseJObject(IEnumerable<JToken> tokens, Guid? parentId = null, bool array = false)
        {
            foreach (JProperty property in tokens)
            {
                string name = property.Name;
                switch (property.Value.Type)
                {
                    case JTokenType.Object:
                        await ParseJObject(property.Value.Children(), await CreateOutputDataSchema(name, DataTypeEnum.Object, parentId, array));
                        break;
                    
                    case JTokenType.String:
                        await CreateOutputDataSchema(name, DataTypeEnum.String, parentId, array);
                        break;
                    
                    case JTokenType.Float:
                    case JTokenType.Integer:
                        await CreateOutputDataSchema(name, DataTypeEnum.Number, parentId, array);
                        break;

                    case JTokenType.Boolean:
                        await CreateOutputDataSchema(name, DataTypeEnum.Bool, parentId, array);
                        break;

                    case JTokenType.Array:
                        await ParseJArray(name, property, parentId, array);
                        break;
                }
            }
        }

        private async Task ParseJArray(string? name, JProperty property, Guid? parentId = null, bool array = false)
        {
            JToken? arrayItem = property.Value.Children().FirstOrDefault();
            if (arrayItem != null)
            {
                switch (property.Value.Type)
                {
                    case JTokenType.String:
                        await CreateOutputDataSchema(name, DataTypeEnum.ArrayString, parentId, array);
                        break;

                    case JTokenType.Float:
                    case JTokenType.Integer:
                        await CreateOutputDataSchema(name, DataTypeEnum.ArrayNumber, parentId, array);
                        break;

                    case JTokenType.Boolean:
                        await CreateOutputDataSchema(name, DataTypeEnum.ArrayBool, parentId, array);
                        break;

                    // TODO
                    /*case JTokenType.Object: 
                        await ParseJObject(property.Value.Children(), await CreateOutputDataSchema(null, DataTypeEnum.ArrayObject, parentId, array), true);
                        break;

                    case JTokenType.Array:
                        await ParseJArray(null, property, await CreateOutputDataSchema(null, DataTypeEnum.ArrayArray, parentId, array), true);
                        break;*/
                }   
            }
        }
    }
}
