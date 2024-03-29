using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Account;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Attribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockModel.ConfigTypes;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.System;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class BlockModelFacade : BaseFacade
    {
        private readonly BlockModelRepository _blockModelRepository;
        private readonly AttributeRepository _attributeRepository;
        private readonly AttributeMapRepository _attributeMapRepository;
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly DataSchemaRepository _dataSchemaRepository;
        private readonly DataSchemaMapRepository _dataSchemaMapRepository;
        private readonly FlowRepository _flowRepository;
        private readonly BlockModelDataSchemaRepository _blockModelDataSchemaRepository;
        private readonly AgendaRepository _agendaRepository;
        private readonly ForeignSendEventRepository _foreignSendEventRepository;
        private readonly ModelRepository _modelRepository;
        private readonly ForeignRecieveEventRepository _foreignRecieveEventRepository;
        private readonly ForeignAttributeMapRepository _foreignAttributeMapRepository;
        private readonly IMapper _mapper;
        private Guid _blockId;

        public BlockModelFacade(BlockModelRepository blockModelRepository, AttributeRepository attributeRepository,
                                AttributeMapRepository attributeMapRepository, PoolRepository poolRepository,
                                SystemRepository systemRepository, ServiceRepository serviceRepository, 
                                DataSchemaRepository dataSchemaRepository, DataSchemaMapRepository dataSchemaMapRepository, 
                                FlowRepository flowRepository, BlockModelDataSchemaRepository blockModelDataSchemaRepository, 
                                AgendaRepository agendaRepository, ForeignSendEventRepository foreignSendEventRepository, 
                                ModelRepository modelRepository, ForeignRecieveEventRepository foreignRecieveEventRepository, 
                                FilterRepository filterRepository, ForeignAttributeMapRepository foreignAttributeMapRepository, 
                                IMapper mapper)
        : base(filterRepository)
        {
            _blockModelRepository = blockModelRepository;
            _attributeRepository = attributeRepository;
            _attributeMapRepository = attributeMapRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _serviceRepository = serviceRepository;
            _dataSchemaRepository = dataSchemaRepository;
            _dataSchemaMapRepository = dataSchemaMapRepository;
            _flowRepository = flowRepository;
            _blockModelDataSchemaRepository = blockModelDataSchemaRepository;
            _agendaRepository = agendaRepository;
            _foreignSendEventRepository = foreignSendEventRepository;
            _foreignRecieveEventRepository = foreignRecieveEventRepository;
            _modelRepository = modelRepository;
            _foreignAttributeMapRepository = foreignAttributeMapRepository;
            _mapper = mapper;
        }

        public async Task<List<AgendaIdNameDTO>> Agendas(Guid systemId)
        {
            if (systemId == StaticData.ThisSystemId)
            {
                return await _agendaRepository.Agendas();
            }
            else
            {
                DstAddressDTO address = await _systemRepository.Address(systemId);
                return await CommunicationHelper.Agendas(address);
            }
        }

        public async Task<ServiceTaskModelConfigDTO> AddMap(Guid serviceTaskId, Guid sourceId, Guid targetId)
        {
            if (!await _dataSchemaMapRepository.Any(serviceTaskId, sourceId, targetId))
            {
                await _dataSchemaMapRepository.Create(new DataSchemaMapEntity
                {
                    ServiceTaskId = serviceTaskId,
                    SourceId = sourceId,
                    TargetId = targetId
                });
                await _dataSchemaMapRepository.Save();
            }

            IServiceTaskModelEntity serviceTask = await _blockModelRepository.Bare(serviceTaskId) as IServiceTaskModelEntity;
            ServiceTaskModelConfigDTO dto = new ServiceTaskModelConfigDTO();
            dto.MappedSchemas = await _dataSchemaMapRepository.Mapped(serviceTask.Id);
            dto.TargetSchemas = await _dataSchemaRepository.Targets(serviceTask.ServiceId, serviceTaskId);
            dto.SourceSchemas = await _blockModelRepository.Sources(serviceTask.Order, serviceTask.PoolId);
            dto.CurrentServiceId = serviceTask.ServiceId;
            return dto;
        }

        public async Task<ServiceTaskModelConfigDTO> RemoveMap(Guid serviceTaskId, Guid sourceId, Guid targetId)
        {
            _dataSchemaMapRepository.Remove(new DataSchemaMapEntity
            {
                ServiceTaskId = serviceTaskId,
                SourceId = sourceId,
                TargetId = targetId
            });
            await _dataSchemaMapRepository.Save();

            IServiceTaskModelEntity serviceTask = await _blockModelRepository.Bare(serviceTaskId) as IServiceTaskModelEntity;
            ServiceTaskModelConfigDTO dto = new ServiceTaskModelConfigDTO();
            dto.MappedSchemas = await _dataSchemaMapRepository.Mapped(serviceTask.Id);
            dto.TargetSchemas = await _dataSchemaRepository.Targets(serviceTask.ServiceId, serviceTaskId);
            dto.SourceSchemas = await _blockModelRepository.Sources(serviceTask.Order, serviceTask.PoolId);
            dto.CurrentServiceId = serviceTask.ServiceId;
            return dto;
        }

        public async Task<BlockModelConfigDTO> SenderChange(SenderChangeDTO dto)
        {
            IRecieveSignalEventModelEntity entity = await _blockModelRepository.Bare(dto.Id) as IRecieveSignalEventModelEntity;
            if (entity.ForeignSenderId != null)
            {
                SenderRecieverAddressDTO recieverAddress = await _foreignSendEventRepository.SenderAddress(entity.ForeignSenderId.Value);
                await CommunicationHelper.RemoveReciever(recieverAddress, dto.Id, recieverAddress.ForeignBlockId);
                _foreignSendEventRepository.Remove(await _foreignSendEventRepository.ForRemoval(entity.ForeignSenderId.Value));
            }
            
            DstAddressDTO address = await _systemRepository.Address(dto.SystemId);
            List<AttributeEntity> attributes = await CommunicationHelper.AddReciever(address, dto.Id, dto.BlockId);
            
            ForeignSendSignalEventEntity foreignEntity = new ForeignSendSignalEventEntity
            {
                ForeignBlockId = dto.BlockId,
                SystemId = dto.SystemId
            };
            await _foreignSendEventRepository.Create(foreignEntity);
            entity.ForeignSenderId = foreignEntity.Id;

            foreach(AttributeEntity attribute in attributes)
            {
                ForeignAttributeMapEntity map = new ForeignAttributeMapEntity
                {
                    ForeignSendEventId = foreignEntity.Id,
                    ForeignAttributeId = attribute.Id,
                    AttributeId = Guid.NewGuid()
                };

                attribute.Id = map.AttributeId;
                attribute.BlockId = entity.Id;

                await _attributeRepository.Create(attribute);
                await _foreignAttributeMapRepository.Create(map);
            }

            await _blockModelRepository.Save();

            return await Config(dto.Id);
        }

        public async Task<List<BlockIdNameDTO>> SenderBlocks(Guid systemId, Guid poolId)
        {
            if (systemId == StaticData.ThisSystemId)
            {
                return await _blockModelRepository.SenderBlocks(poolId);
            }
            else
            {
                DstAddressDTO address = await _systemRepository.Address(systemId);
                return await CommunicationHelper.SenderBlocks(address, poolId);
            }
        }

        public async Task<List<PoolIdNameDTO>> Pools(Guid systemId, Guid modelId)
        {
            if (systemId == StaticData.ThisSystemId)
            {
                return await _poolRepository.Pools(modelId);
            }
            else
            {
                DstAddressDTO address = await _systemRepository.Address(systemId);
                return await CommunicationHelper.Pools(address, modelId);
            }
        }

        public async Task<List<ModelIdNameDTO>> Models(Guid systemId, Guid agendaId)
        {
            if (systemId == StaticData.ThisSystemId)
            {
                return await _modelRepository.Models(agendaId);
            }
            else
            {
                DstAddressDTO address = await _systemRepository.Address(systemId);
                return await CommunicationHelper.Models(address, agendaId);
            }
        }

        public async Task<List<SystemIdNameDTO>> ChangeSender(Guid modelId)
        {
            List<SystemIdNameDTO> systems = await _systemRepository.ThisSystemIdName();
            systems.AddRange(await _modelRepository.Systems(modelId));
            return systems;
        }

        public async Task Edit(BlockModelEditDTO dto)
        {
            BlockModelEntity block = await _blockModelRepository.Detail(dto.Id);
            block.Description = dto.Description;

            if (block is IServiceTaskModelEntity)
            {
                (block as IServiceTaskModelEntity).ServiceId = dto.ServiceId;
            }
            else if (block is IUserTaskModelEntity)
            {
                (block as IUserTaskModelEntity).Difficulty = dto.Difficulty;
            }

            await _blockModelRepository.Save();
        }

        public async Task RemoveAttribute(Guid id)
        {
            AttributeEntity attrib = await _attributeRepository.ForRemoval(id);
            
            foreach (BlockModelEntity mappedBlock in attrib.MappedBlocks.Select(x => x.Block))
            {
                if (mappedBlock is ISendMessageEventModelEntity)
                {
                    BlockAddressDTO recieverAddress = await _blockModelRepository.RecieverAddress(mappedBlock.Id);
                    await CommunicationHelper.RemoveRecieverAttribute(recieverAddress, id);
                }
                else if (mappedBlock is ISendSignalEventModelEntity)
                {
                    foreach (SenderRecieverAddressDTO recieverAddress in await _blockModelRepository.ForeignRecieverAddresses(mappedBlock.Id))
                    {
                        await CommunicationHelper.RemoveForeignRecieverAttribute(recieverAddress, id);
                    }
                }
            }

            if (attrib.Data.Count > 0)
            {
                attrib.Disabled = true;
            }
            else
            {
                _attributeRepository.Remove(attrib);
            }
            await _attributeRepository.Save();
        }

        public async Task<BlockModelConfigDTO> CreateEdit(AttributeCreateEditDTO dto)
        {
            AttributeEntity entity = _mapper.Map<AttributeEntity>(dto);
            if (dto.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                await _attributeRepository.Create(entity);
            }
            else
            {
                List<BlockModelEntity?> blocks = await _attributeMapRepository.MappedBlocks(dto.Id);
                foreach (BlockModelEntity block in blocks.Where(x => x is ISendMessageEventModelEntity))
                {
                    BlockAddressDTO recieverAddress = await _blockModelRepository.RecieverAddress(block.Id);
                    entity.BlockId = recieverAddress.BlockId;
                    await CommunicationHelper.UpdateRecieverAttribute(recieverAddress, entity);
                }

                foreach (BlockModelEntity block in blocks.Where(x => x is ISendSignalEventModelEntity))
                {
                    foreach (SenderRecieverAddressDTO recieverAddress in await _blockModelRepository.ForeignRecieverAddresses(block.Id))
                    {
                        entity.BlockId = recieverAddress.ForeignBlockId;
                        await CommunicationHelper.UpdateForeignRecieverAttribute(recieverAddress, entity);
                    }
                }

                entity.BlockId = dto.BlockId;
                _attributeRepository.Update(entity);
            }

            await _attributeRepository.Save();
            
            return await Config(dto.BlockId);
        }

        public async Task ToggleTaskMap(Guid blockId, Guid attributeId)
        {
            AttributeMapEntity entity = new AttributeMapEntity()
            {
                AttributeId = attributeId,
                BlockId = blockId
            };

            if (await _attributeMapRepository.Any(blockId, attributeId))
            {
                _attributeMapRepository.Remove(entity);
            }
            else
            {   
                await _attributeMapRepository.Create(entity);
            }

            await _attributeMapRepository.Save();
        }

        public async Task<bool> ToggleSendMap(Guid blockId, Guid attributeId)
        {
            AttributeEntity attrib = await _attributeRepository.Bare(attributeId);
            Guid startBlockId = attrib.BlockId;

            bool remove = await _attributeMapRepository.Any(blockId, attributeId);
            bool success = true;
            if (remove)
            {
                BlockAddressDTO? recieverAddress = await _blockModelRepository.RecieverAddress(blockId);
                if (recieverAddress != null)
                {
                    success &= await CommunicationHelper.RemoveRecieverAttribute(recieverAddress, attributeId);
                }

                foreach (SenderRecieverAddressDTO recvAddress in await _blockModelRepository.ForeignRecieverAddresses(blockId))
                {
                    success &= await CommunicationHelper.RemoveForeignRecieverAttribute(recvAddress, attributeId);
                }
            }
            else
            {
                BlockAddressDTO? recieverAddress = await _blockModelRepository.RecieverAddress(blockId);
                if (recieverAddress != null)
                {
                    attrib.BlockId = recieverAddress.BlockId;
                    success &= await CommunicationHelper.CreateRecieverAttribute(recieverAddress, attrib);
                }

                foreach (SenderRecieverAddressDTO recvAddress in await _blockModelRepository.ForeignRecieverAddresses(blockId))
                {
                    attrib.BlockId = recvAddress.ForeignBlockId;
                    success &= await CommunicationHelper.CreateForeignRecieverAttribute(recvAddress, attrib);
                }
            }

            attrib.BlockId = startBlockId;
            if (success)
            {
                AttributeMapEntity entity = new AttributeMapEntity()
                {
                    AttributeId = attributeId,
                    BlockId = blockId
                };

                if (remove)
                {
                    _attributeMapRepository.Remove(entity);
                }
                else
                {   
                    await _attributeMapRepository.Create(entity);
                }

                await _attributeMapRepository.Save();
            }

            return success;
        }

        public async Task ToggleServiceMap(Guid blockId, Guid dataSchemaId, Guid serviceTaskId)
        {
            BlockModelDataSchemaEntity entity = new BlockModelDataSchemaEntity() 
            {
                BlockId = blockId,
                DataSchemaId = dataSchemaId,
                ServiceTaskId = serviceTaskId
            };

            if (await _blockModelDataSchemaRepository.Any(blockId, dataSchemaId, serviceTaskId))
            {
                _blockModelDataSchemaRepository.Remove(entity);
            }
            else
            {
                await _blockModelDataSchemaRepository.Create(entity);
            }

            await _blockModelDataSchemaRepository.Save();
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
                    dto = new BlockModelConfigDTO();
                    break;

                case IEndEventModelEntity endEvent:
                    dto = new BlockModelConfigDTO();
                    break;
                
                case IExclusiveGatewayModelEntity exclusiveGateway:
                    dto = new BlockModelConfigDTO();
                    break;

                case IParallelGatewayModelEntity parallelGateway:
                    dto = new BlockModelConfigDTO();
                    break;

                case ISendMessageEventModelEntity sendMessageEvent:
                    dto = await SendMessageEventConfig(sendMessageEvent);
                    break;
                
                case ISendSignalEventModelEntity sendSignalEvent:
                    dto = await SendSignalEventConfig(sendSignalEvent);
                    break;

                case IRecieveMessageEventModelEntity recieveMessageEvent:
                    dto = await RecieveMessageEventConfig(recieveMessageEvent);
                    break;
                
                case IRecieveSignalEventModelEntity recieveSignalEvent:
                    dto = await RecieveSignalEventConfig(recieveSignalEvent);
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

        private async Task<BlockModelConfigDTO> RecieveMessageEventConfig(IRecieveMessageEventModelEntity recieveEvent)
        {
            RecieveMessageEventModelConfigDTO dto = new RecieveMessageEventModelConfigDTO();
            dto.OutputAttributes = await _attributeRepository.OutputAttributes(recieveEvent.Id);
            dto.Sender = await _blockModelRepository.SenderMessageInfo(recieveEvent.Id);

            return dto;
        }

        private async Task<BlockModelConfigDTO> RecieveSignalEventConfig(IRecieveSignalEventModelEntity recieveEvent)
        {
            RecieveSignalEventModelConfigDTO dto = new RecieveSignalEventModelConfigDTO();
            dto.OutputAttributes = await _attributeRepository.OutputAttributes(recieveEvent.Id);

            if (recieveEvent.ForeignSenderId != null)
            {
                SenderRecieverAddressDTO address = await _foreignSendEventRepository.SenderAddress(recieveEvent.ForeignSenderId.Value);
                dto.Sender = await CommunicationHelper.SenderInfo(address, address.ForeignBlockId);
                dto.Sender.SystemName = address.SystemName;
            }
            else
            {
                dto.Sender = new SenderRecieverConfigDTO();
            }

            return dto;
        }

        private async Task<BlockModelConfigDTO> SendMessageEventConfig(ISendMessageEventModelEntity sendEvent)
        {
            SendMessageEventModelConfigDTO dto = new SendMessageEventModelConfigDTO();
            dto.InputAttributes = await _blockModelRepository.TaskInputAttributes(sendEvent.Id, sendEvent.Order, sendEvent.PoolId);
            dto.Reciever = await _blockModelRepository.RecieversInfo(sendEvent.Id);

            return dto;
        }

        private async Task<BlockModelConfigDTO> SendSignalEventConfig(ISendSignalEventModelEntity sendEvent)
        {
            SendSignalEventModelConfigDTO dto = new SendSignalEventModelConfigDTO();
            dto.InputAttributes = await _blockModelRepository.TaskInputAttributes(sendEvent.Id, sendEvent.Order, sendEvent.PoolId);
            foreach (SenderRecieverAddressDTO address in await _blockModelRepository.ForeignRecieverAddresses(sendEvent.Id))
            {
                try
                {
                    SenderRecieverConfigDTO senderReciever = await CommunicationHelper.ForeignRecieverInfo(address, address.ForeignBlockId);
                    senderReciever.SystemName = address.SystemName;
                    dto.Recievers.Add(senderReciever);
                }
                catch {}
            }

            return dto;
        }

        private async Task<BlockModelConfigDTO> UserTaskConfig(IUserTaskModelEntity userTask)
        {
            UserTaskModelConfigDTO dto = new UserTaskModelConfigDTO();
            dto.OutputAttributes = await _attributeRepository.OutputAttributes(userTask.Id);
            dto.InputAttributes = await _blockModelRepository.TaskInputAttributes(userTask.Id, userTask.Order, userTask.PoolId);
            dto.InputAttributes.AddRange(await _blockModelRepository.RecieveSignalEventAttribures(userTask.Id, userTask.Order, userTask.PoolId));
            dto.InputAttributes.AddRange(await _blockModelRepository.RecieveMessageEventAttribures(userTask.Id, userTask.Order, userTask.PoolId));
            
            (dto as IDifficultyConfig).Difficulty = userTask.Difficulty;

            dto.ServiceInputAttributes = await _blockModelRepository.ServiceInputAttributes(userTask.Id, userTask.Order, userTask.PoolId);
            dto.ServiceOutputAttributes = await _blockModelRepository.ServiceOutputAttributes(userTask.Id, userTask.Order, userTask.PoolId);

            return dto;
        }

        private async Task<BlockModelConfigDTO> ServiceTaskConfig(IServiceTaskModelEntity serviceTask)
        {
            ServiceTaskModelConfigDTO dto = new ServiceTaskModelConfigDTO();
            dto.CurrentServiceId = serviceTask.ServiceId;
            dto.Services.Add(new ServiceIdNameDTO
            {
                Id = null,
                Name = "Nevybrána"
            });
            dto.Services.AddRange(await _serviceRepository.AllIdNames());

            dto.MappedSchemas = await _dataSchemaMapRepository.Mapped(serviceTask.Id);
            dto.TargetSchemas = await _dataSchemaRepository.Targets(serviceTask.ServiceId, serviceTask.Id);
            dto.SourceSchemas = await _blockModelRepository.Sources(serviceTask.Order, serviceTask.PoolId);

            return dto;
        }
    }
}
