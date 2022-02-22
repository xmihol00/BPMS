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
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockModel.ConfigTypes;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.System;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class BlockModelFacade
    {
        private readonly BlockModelRepository _blockModelRepository;
        private readonly BlockAttributeRepository _blockAttributeRepository;
        private readonly BlockAttributeMapRepository _blockAttributeMapRepository;
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly ServiceDataSchemaRepository _dataSchemaRepository;
        private readonly FlowRepository _flowRepository;
        private readonly IMapper _mapper;
        private Guid _blockId;
        private bool _deepAttributeSearch = false;
        private bool _compulsoryAttributes = true;

        public BlockModelFacade(BlockModelRepository blockModelRepository, BlockAttributeRepository blockAttributeRepository,
                                BlockAttributeMapRepository blockAttributeMapRepository, PoolRepository poolRepository,
                                SystemRepository systemRepository, ServiceRepository serviceRepository, 
                                ServiceDataSchemaRepository dataSchemaRepository, FlowRepository flowRepository,
                                IMapper mapper)
        {
            _blockModelRepository = blockModelRepository;
            _blockAttributeRepository = blockAttributeRepository;
            _blockAttributeMapRepository = blockAttributeMapRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _serviceRepository = serviceRepository;
            _dataSchemaRepository = dataSchemaRepository;
            _flowRepository = flowRepository;
            _mapper = mapper;
        }

        public async Task Edit(BlockModelEditDTO dto)
        {
            BlockModelEntity block = await _blockModelRepository.Detail(dto.Id);
            block.Name = dto.Name;
            block.Description = dto.Description ?? "";

            if (block is IServiceTaskModelEntity)
            {
                (block as IServiceTaskModelEntity).ServiceId = dto.ServiceId;
                (block as IServiceTaskModelEntity).RoleId = dto.RoleId;
            }
            else if (block is IUserTaskModelEntity)
            {
                (block as IUserTaskModelEntity).RoleId = dto.RoleId;
            }

            await _blockModelRepository.Save();
        }

        public async Task Remove(Guid id)
        {
            _blockAttributeRepository.Remove(new BlockAttributeEntity { Id = id });
            await _blockAttributeRepository.Save();
        }

        public async Task<BlockModelConfigDTO> CreateEdit(AttributeCreateEditDTO dto)
        {
            BlockAttributeEntity entity = _mapper.Map<BlockAttributeEntity>(dto);
            if (dto.Id == Guid.Empty)
            {
                await _blockAttributeRepository.Create(entity);
            }
            else
            {
                _blockAttributeRepository.Update(entity);
            }

            await _blockAttributeRepository.Save();
            
            return await Config(dto.BlockId);
        }

        public async Task ToggleMap(Guid blockId, Guid attributeId)
        {
            BlockAttributeMapEntity entity = new BlockAttributeMapEntity()
            {
                AttributeId = attributeId,
                BlockId = blockId
            };

            if (await _blockAttributeMapRepository.Any(blockId, attributeId))
            {
                _blockAttributeMapRepository.Remove(entity);

                BlockTypePoolIdDTO typePoolId = await _blockModelRepository.BlockTypePoolId(blockId);
                if (typePoolId.Type == typeof(SendEventModelEntity))
                {
                    foreach (BlockAttributeMapEntity map in await _blockAttributeRepository.MapsFromDifferentPool(attributeId, typePoolId.PoolId))
                    {
                        _blockAttributeMapRepository.Remove(entity);
                    }
                }
            }
            else
            {   
                await _blockAttributeMapRepository.Create(entity);
            }

            await _blockAttributeMapRepository.Save();
        }

        public async Task<BlockModelConfigDTO> Config(Guid id)
        {
            BlockModelEntity entity = await _blockModelRepository.Config(id);
            _blockId = entity.Id;

            BlockModelConfigDTO dto;
            switch (entity)
            {
                case IUserTaskModelEntity userTask:
                    dto = await UserTaskConfig(userTask);
                    break;

                case IServiceTaskModelEntity serviceTask:
                    dto = await ServiceTaskConfig(serviceTask);
                    break;

                case IStartEventModelEntity startEvent:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;

                case IEndEventModelEntity endEvent:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;
                
                case IExclusiveGatewayModelEntity exclusiveGateway:
                    dto = new BlockModelConfigDTO();
                    break;

                case IParallelGatewayModelEntity parallelGateway:
                    dto = new BlockModelConfigDTO();
                    break;

                case ISendEventModelEntity sendEvent:
                    dto = await SendEventConfig(sendEvent);
                    break;

                case IRecieveEventModelEntity recieveEvent:
                    dto = await RecieveEventConfig(recieveEvent);
                    break;

                default:
                    dto = new BlockModelConfigDTO();
                    break;
            }

            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.Description = entity.Description;

            return dto;
        }

        private async Task<BlockModelConfigDTO> RecieveEventConfig(IRecieveEventModelEntity recieveEvent)
        {
            BlockModelConfigDTO dto = new RecieveEventConfigDTO();
            (dto as IRecievedMessageConfig).Message = await RecievedMessage(recieveEvent);
            return dto;
        }

        private async Task<BlockModelConfigDTO> SendEventConfig(ISendEventModelEntity sendEvent)
        {
            BlockModelConfigDTO dto = new SendEventConfigDTO();
            _deepAttributeSearch = true;
            (dto as IInputAttributesConfig).InputAttributes = await InputAttributes(sendEvent.InFlows);
            return dto;
        }

        private async Task<BlockModelConfigDTO> UserTaskConfig(IUserTaskModelEntity userTask)
        {
            BlockModelConfigDTO dto = new UserTaskConfigDTO();
            (dto as IAttributesConfig).Attributes = await _blockAttributeRepository.Details(userTask.Id);
            (dto as IInputAttributesConfig).InputAttributes = await InputAttributes(userTask.InFlows);
            IRoleConfig roleConfig = dto as IRoleConfig;
            roleConfig.CurrentRole = userTask.RoleId;
            roleConfig.Roles.Add(new RoleAllDTO
            {
                Id = null,
                Name = "Nevybrána",
            });
            roleConfig.Roles.AddRange(await _poolRepository.RolesOfAgenda(userTask.PoolId));

            (dto as IServiceOutputAttributes).ServiceOutputAttributes = await FindOutputService(userTask.InFlows);
            (dto as IServiceInputAttributes).ServiceInputAttributes = await FindInputService(userTask.OutFlows);

            return dto;
        }

        private async Task<List<DataSchemaAttributeDTO>> FindInputService(List<FlowEntity> flows)
        {
            List<DataSchemaAttributeDTO> attribs = new List<DataSchemaAttributeDTO>();
            foreach (FlowEntity flow in flows)
            {
                if (flow.InBlock is IServiceTaskModelEntity)
                {
                    attribs.AddRange(await _dataSchemaRepository.AsAttributes((flow.InBlock as IServiceTaskModelEntity).ServiceId, 
                                                                               DirectionEnum.Input));
                }
                else if (flow.InBlock is IRecieveEventModelEntity || flow.InBlock is ISendEventModelEntity)
                {
                    attribs.AddRange(await FindInputService(await _flowRepository.OutFlows(flow.InBlockId)));
                }
            }

            return attribs;
        }

        private async Task<List<DataSchemaAttributeDTO>> FindOutputService(List<FlowEntity> flows)
        {
            List<DataSchemaAttributeDTO> attribs = new List<DataSchemaAttributeDTO>();
            foreach (FlowEntity flow in flows)
            {
                if (flow.OutBlock is IServiceTaskModelEntity)
                {
                    attribs.AddRange(await _dataSchemaRepository.AsAttributes((flow.OutBlock as IServiceTaskModelEntity).ServiceId, 
                                                                              DirectionEnum.Output));
                }
                else if (flow.OutBlock is IRecieveEventModelEntity || flow.OutBlock is ISendEventModelEntity)
                {
                    attribs.AddRange(await FindOutputService(await _flowRepository.InFlows(flow.OutBlockId)));
                }
            }

            return attribs;
        }

        private async Task<BlockModelConfigDTO> ServiceTaskConfig(IServiceTaskModelEntity serviceTask)
        {
            BlockModelConfigDTO dto = new ServiceTaskConfigDTO();
            IServiceConfig service = dto as IServiceConfig;
            service.CurrentService = serviceTask.ServiceId;
            service.Services.Add(new ServiceIdNameDTO
            {
                Id = null,
                Name = "Nevybrána"
            });
            service.Services.AddRange(await _serviceRepository.AllIdNames());

            IRoleConfig roleConfig = dto as IRoleConfig;
            roleConfig.CurrentRole = serviceTask.RoleId;
            roleConfig.Roles.Add(new RoleAllDTO 
            {
                Id = null,
                Name = "Nevybrána",
            });
            roleConfig.Roles.AddRange(await _poolRepository.RolesOfAgenda(serviceTask.PoolId));

            return dto;
        }

        private async Task<RecievedMessageDTO> RecievedMessage(IRecieveEventModelEntity entity)
        {
            RecievedMessageDTO dto = new RecievedMessageDTO();
            SystemIdAgendaIdDTO ids = await _poolRepository.CurrentSystemIdAgendaId(entity.PoolId);
            dto.CurrentSystemId = ids.SystemId;
            dto.Editable = entity.Editable;
            dto.Attributes = await _blockModelRepository.MappedAttributes(entity.SenderId);
            
            if (entity.Editable)
            {
                dto.Systems = await _systemRepository.SystemsOfAgenda(ids.AgendaId);
            }

            return dto;
        }

        private async Task<List<IGrouping<string, InputBlockAttributeDTO>>> InputAttributes(List<FlowEntity> flows)
        {
            List<IGrouping<string, InputBlockAttributeDTO>> inputAttributes = new List<IGrouping<string, InputBlockAttributeDTO>>();
            foreach (FlowEntity flow in flows)
            {
                inputAttributes.AddRange(await FlowInputAttributes(flow));
            }

            return inputAttributes;
        }

        private async Task<List<IGrouping<string, InputBlockAttributeDTO>>> FlowInputAttributes(FlowEntity flow)
        {
            List<IGrouping<string, InputBlockAttributeDTO>> attributes = new List<IGrouping<string, InputBlockAttributeDTO>>();
            
            BlockModelEntity block = await _blockModelRepository.PreviousFlow(flow.OutBlockId);

            if (block is INoAttributes)
            {
                return attributes;
            }

            if (block is IAttributes)
            {
                attributes.AddRange(await _blockAttributeRepository.InputAttributes(_blockId, flow.OutBlockId, _compulsoryAttributes));
                if (!_deepAttributeSearch)
                {
                    return attributes;
                }
            }

            if (block is IRecieveEventModelEntity)
            {
                attributes.AddRange(
                    await _blockModelRepository.MappedInputAttributes(_blockId, (block as IRecieveEventModelEntity).SenderId, 
                                                                      block.Name, _compulsoryAttributes));
                if (!_deepAttributeSearch)
                {
                    return attributes;
                }
            }

            if (block is IExclusiveGatewayModelEntity)
            {
                _compulsoryAttributes = true;
            }

            foreach (FlowEntity nextFlow in block.InFlows)
            {
                attributes.AddRange(await FlowInputAttributes(nextFlow));
            }
            return attributes;
        }

        public Task<object?> PoolConfig(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
