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
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Account;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockModel.ConfigTypes;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.Pool;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.System;
using BPMS_DTOs.User;
using Newtonsoft.Json;

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
        private readonly BlockModelDataSchemaRepository _blockModelDataSchemaRepository;
        private readonly ForeignSendEventRepository _foreignSendEventRepository;
        private readonly ForeignRecieveEventRepository _foreignRecieveEventRepository;
        private readonly IMapper _mapper;
        private Guid _blockId;

        public BlockModelFacade(BlockModelRepository blockModelRepository, BlockAttributeRepository blockAttributeRepository,
                                BlockAttributeMapRepository blockAttributeMapRepository, PoolRepository poolRepository,
                                SystemRepository systemRepository, ServiceRepository serviceRepository, 
                                ServiceDataSchemaRepository dataSchemaRepository, FlowRepository flowRepository,
                                BlockModelDataSchemaRepository blockModelDataSchemaRepository, 
                                ForeignSendEventRepository foreignSendEventRepository, 
                                ForeignRecieveEventRepository foreignRecieveEventRepository, IMapper mapper)
        {
            _blockModelRepository = blockModelRepository;
            _blockAttributeRepository = blockAttributeRepository;
            _blockAttributeMapRepository = blockAttributeMapRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _serviceRepository = serviceRepository;
            _dataSchemaRepository = dataSchemaRepository;
            _flowRepository = flowRepository;
            _blockModelDataSchemaRepository = blockModelDataSchemaRepository;
            _foreignSendEventRepository = foreignSendEventRepository;
            _foreignRecieveEventRepository = foreignRecieveEventRepository;
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
            BlockAttributeEntity attrib = new BlockAttributeEntity()
            { 
                Id = id,
                MappedBlocks = await _blockAttributeRepository.MappedBlocks(id)
            };

            foreach (BlockModelEntity mappedBlock in attrib.MappedBlocks.Select(x => x.Block))
            {
                if (mappedBlock is ISendEventModelEntity)
                {
                    foreach (BlockAddressDTO recieverAddress in await _poolRepository.RecieverAddresses(mappedBlock.Id))
                    {
                        await CommunicationHelper.RemoveRecieverAttribute(recieverAddress.DestinationURL, 
                                                                          SymetricCipherHelper.JsonEncrypt(recieverAddress),
                                                                          id.ToString());
                    }
                }
            }

            _blockAttributeRepository.Remove(attrib);
            await _blockAttributeRepository.Save();
        }

        public async Task<BlockModelConfigDTO> CreateEdit(AttributeCreateEditDTO dto)
        {
            BlockAttributeEntity entity = _mapper.Map<BlockAttributeEntity>(dto);
            if (dto.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                await _blockAttributeRepository.Create(entity);
            }
            else
            {
                _blockAttributeRepository.Update(entity);
            }

            await _blockAttributeRepository.Save();
            
            return await Config(dto.BlockId);
        }

        public async Task ToggleTaskMap(Guid blockId, Guid attributeId)
        {
            BlockAttributeMapEntity entity = new BlockAttributeMapEntity()
            {
                AttributeId = attributeId,
                BlockId = blockId
            };

            if (await _blockAttributeMapRepository.Any(blockId, attributeId))
            {
                _blockAttributeMapRepository.Remove(entity);
            }
            else
            {   
                await _blockAttributeMapRepository.Create(entity);
            }

            await _blockAttributeMapRepository.Save();
        }

        public async Task ToggleSendMap(Guid blockId, Guid attributeId)
        {
            await ToggleTaskMap(blockId, attributeId);

            BlockAttributeEntity attrib = await _blockAttributeRepository.Bare(attributeId);

            bool success = true;
            foreach (BlockAddressDTO recieverAddress in await _poolRepository.RecieverAddresses(blockId))
            {
                attrib.BlockId = recieverAddress.BlockId;
                success &= await CommunicationHelper.ToggleRecieverAttribute(recieverAddress.DestinationURL, 
                                                                          SymetricCipherHelper.JsonEncrypt(recieverAddress),
                                                                          JsonConvert.SerializeObject(attrib));
            }

            // TODO success
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
            dto.OutputAttributes = await _blockAttributeRepository.Details(recieveEvent.Id);

            if (recieveEvent.SenderId != null)
            {
                dto.Sender = await _blockModelRepository.SenderInfo(recieveEvent.SenderId.Value, false);
                return dto;
            }
            if (recieveEvent.ForeignSenderId != null)
            {
                SenderRecieverAddressDTO address = await _foreignSendEventRepository.SenderAddress(recieveEvent.ForeignSenderId.Value);
                dto.Sender = await CommunicationHelper.SenderInfo(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address),
                                                                  address.ForeignBlockId.ToString());

                return dto;
            }

            return dto;
        }

        private async Task<BlockModelConfigDTO> SendEventConfig(ISendEventModelEntity sendEvent)
        {
            SendEventModelConfigDTO dto = new SendEventModelConfigDTO();
            dto.InputAttributes = await _blockModelRepository.TaskInputAttributes(sendEvent.Id, sendEvent.Order, sendEvent.PoolId);
            dto.Recievers = await _blockModelRepository.RecieversInfo(sendEvent.Id);
            foreach (SenderRecieverAddressDTO address in await _blockModelRepository.RecieverAddresses(sendEvent.Id))
            {
                try
                {
                    dto.Recievers.AddRange(await CommunicationHelper.RecieversInfo(address.DestinationURL, 
                                                                                 SymetricCipherHelper.JsonEncrypt(address),
                                                                                 address.ForeignBlockId.ToString()));
                }
                catch {}
            }

            return dto;
        }

        private async Task<BlockModelConfigDTO> UserTaskConfig(IUserTaskModelEntity userTask)
        {
            UserTaskModelConfigDTO dto = new UserTaskModelConfigDTO();
            dto.OutputAttributes = await _blockAttributeRepository.Details(userTask.Id);
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
