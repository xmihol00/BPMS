using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
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
    public class BlockModelFacade
    {
        private readonly BlockModelRepository _blockModelRepository;
        private readonly AttributeRepository _attributeRepository;
        private readonly AttributeMapRepository _attributeMapRepository;
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly DataSchemaRepository _dataSchemaRepository;
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
                                DataSchemaRepository dataSchemaRepository, FlowRepository flowRepository,
                                BlockModelDataSchemaRepository blockModelDataSchemaRepository, AgendaRepository agendaRepository,
                                ForeignSendEventRepository foreignSendEventRepository, ModelRepository modelRepository,
                                ForeignRecieveEventRepository foreignRecieveEventRepository, 
                                ForeignAttributeMapRepository foreignAttributeMapRepository, IMapper mapper)
        {
            _blockModelRepository = blockModelRepository;
            _attributeRepository = attributeRepository;
            _attributeMapRepository = attributeMapRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _serviceRepository = serviceRepository;
            _dataSchemaRepository = dataSchemaRepository;
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
                return await CommunicationHelper.Agendas(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address));
            }
        }

        public async Task<BlockModelConfigDTO> SenderChange(SenderChangeDTO dto)
        {
            IRecieveEventModelEntity entity = await _blockModelRepository.Bare(dto.Id) as IRecieveEventModelEntity;
            if (entity.ForeignSenderId != null)
            {
                SenderRecieverAddressDTO recieverAddress = await _foreignSendEventRepository.SenderAddress(entity.ForeignSenderId.Value);
                await CommunicationHelper.RemoveReciever(recieverAddress.DestinationURL, SymetricCipherHelper.JsonEncrypt(recieverAddress), 
                                                         dto.Id, recieverAddress.ForeignBlockId);
                _foreignSendEventRepository.Remove(await _foreignSendEventRepository.ForRemoval(entity.ForeignSenderId.Value));
                // TODO check if all is removed
            }
            
            DstAddressDTO address = await _systemRepository.Address(dto.SystemId);
            List<AttributeEntity> attributes = await CommunicationHelper.AddReciever(address.DestinationURL, 
                                                                                     SymetricCipherHelper.JsonEncrypt(address), 
                                                                                     dto.Id, dto.BlockId);
            
            ForeignSendEventEntity foreignEntity = new ForeignSendEventEntity
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
                return await CommunicationHelper.SenderBlocks(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address), poolId);
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
                return await CommunicationHelper.Pools(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address), modelId);
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
                return await CommunicationHelper.Models(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address), agendaId);
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
            AttributeEntity attrib = new AttributeEntity()
            { 
                Id = id,
                MappedBlocks = await _attributeRepository.MappedBlocks(id)
            };

            foreach (BlockModelEntity mappedBlock in attrib.MappedBlocks.Select(x => x.Block))
            {
                if (mappedBlock is ISendEventModelEntity)
                {
                    foreach (BlockAddressDTO recieverAddress in await _blockModelRepository.RecieverAddresses(mappedBlock.Id))
                    {
                        await CommunicationHelper.RemoveRecieverAttribute(recieverAddress.DestinationURL, 
                                                                          SymetricCipherHelper.JsonEncrypt(recieverAddress), id);
                    }

                    foreach (SenderRecieverAddressDTO recieverAddress in await _blockModelRepository.ForeignRecieverAddresses(mappedBlock.Id))
                    {
                        await CommunicationHelper.RemoveForeignRecieverAttribute(recieverAddress.DestinationURL,
                                                                                 SymetricCipherHelper.JsonEncrypt(recieverAddress), id);
                    }
                }
            }

            _attributeRepository.Remove(attrib);
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

        public async Task ToggleSendMap(Guid blockId, Guid attributeId)
        {
            await ToggleTaskMap(blockId, attributeId);

            AttributeEntity attrib = await _attributeRepository.Bare(attributeId);

            bool success = true;
            foreach (BlockAddressDTO recieverAddress in await _blockModelRepository.RecieverAddresses(blockId))
            {
                attrib.BlockId = recieverAddress.BlockId;
                success &= await CommunicationHelper.ToggleRecieverAttribute(recieverAddress.DestinationURL, 
                                                                          SymetricCipherHelper.JsonEncrypt(recieverAddress),
                                                                          JsonConvert.SerializeObject(attrib));
            }

            foreach (SenderRecieverAddressDTO recieverAddress in await _blockModelRepository.ForeignRecieverAddresses(blockId))
            {
                attrib.BlockId = recieverAddress.ForeignBlockId;
                success &= await CommunicationHelper.ToggleForeignRecieverAttribute(recieverAddress.DestinationURL, 
                                                                                    SymetricCipherHelper.JsonEncrypt(recieverAddress),
                                                                                    JsonConvert.SerializeObject(attrib));
            }

            // TODO check success
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
            RecieveEventModelConfigDTO dto = new RecieveEventModelConfigDTO();
            dto.OutputAttributes = await _attributeRepository.Details(recieveEvent.Id);

            if (recieveEvent.SenderId != null)
            {
                dto.Sender = await _blockModelRepository.SenderInfo(recieveEvent.SenderId.Value);
            }
            else if (recieveEvent.ForeignSenderId != null)
            {
                SenderRecieverAddressDTO address = await _foreignSendEventRepository.SenderAddress(recieveEvent.ForeignSenderId.Value);
                dto.Sender = await CommunicationHelper.SenderInfo(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address),
                                                                  address.ForeignBlockId.ToString());
                dto.Sender.SystemName = address.SystemName;
            }
            else
            {
                dto.Sender = new SenderRecieverConfigDTO();
            }

            dto.Sender.Editable = recieveEvent.Editable;
            return dto;
        }

        private async Task<BlockModelConfigDTO> SendEventConfig(ISendEventModelEntity sendEvent)
        {
            SendEventModelConfigDTO dto = new SendEventModelConfigDTO();
            dto.InputAttributes = await _blockModelRepository.TaskInputAttributes(sendEvent.Id, sendEvent.Order, sendEvent.PoolId);
            dto.Recievers = await _blockModelRepository.RecieversInfo(sendEvent.Id);
            foreach (SenderRecieverAddressDTO address in await _blockModelRepository.ForeignRecieverAddresses(sendEvent.Id))
            {
                try
                {
                    SenderRecieverConfigDTO senderReciever = await CommunicationHelper.ForeignRecieverInfo(address.DestinationURL, 
                                                                                   SymetricCipherHelper.JsonEncrypt(address),
                                                                                   address.ForeignBlockId);
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
            dto.OutputAttributes = await _attributeRepository.Details(userTask.Id);
            dto.InputAttributes = await _blockModelRepository.TaskInputAttributes(userTask.Id, userTask.Order, userTask.PoolId);
            dto.InputAttributes.AddRange(await _blockModelRepository.RecieveEventAttribures(userTask.Id, userTask.Order, userTask.PoolId));
            
            IRoleConfig roleConfig = dto as IRoleConfig;
            roleConfig.CurrentRole = userTask.RoleId;
            roleConfig.Roles.Add(new RoleAllDTO
            {
                Id = null,
                Name = "Nevybrána",
            });
            roleConfig.Roles.AddRange(await _poolRepository.RolesOfAgenda(userTask.PoolId));

            dto.ServiceInputAttributes = await _blockModelRepository.ServiceInputAttributes(userTask.Id, userTask.Order, userTask.PoolId);
            dto.ServiceOutputAttributes = await _blockModelRepository.ServiceOutputAttributes(userTask.Id, userTask.Order, userTask.PoolId);

            return dto;
        }

        private async Task<BlockModelConfigDTO> ServiceTaskConfig(IServiceTaskModelEntity serviceTask)
        {
            BlockModelConfigDTO dto = new ServiceTaskModelConfigDTO();
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
    }
}
