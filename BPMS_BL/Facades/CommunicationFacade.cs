using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_BL.Hubs;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Account;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class CommunicationFacade : BaseFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly UserRepository _userRepository;
        private readonly FlowRepository _flowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly SystemAgendaRepository _systemAgendaRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly AttributeRepository _attributeRepository;
        private readonly DataSchemaRepository _dataSchemaRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockWorkflowRepository _blockWorkflowRepository;
        private readonly ForeignSendEventRepository _foreignSendEventRepository;
        private readonly AgendaRepository _agendaRepository;
        private readonly ForeignRecieveEventRepository _foreignRecieveEventRepository;
        private readonly ForeignAttributeMapRepository _foreignAttributeMapRepository;
        private readonly ConnectionRequestRepository _connectionRequestRepository;
        private readonly AuditMessageRepository _auditMessageRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly BpmsDbContext _context;
        private readonly IMapper _mapper;
        private SystemEntity _system;
        private HttpResponse _response;
        private Guid _messageId;

        #pragma warning disable CS8618
        public CommunicationFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                                   BlockModelRepository blockModelRepository, PoolRepository poolRepository, SystemRepository systemRepository, 
                                   SystemAgendaRepository systemAgendaRepository, WorkflowRepository workflowRepository,
                                   AgendaRoleRepository agendaRoleRepository, AttributeRepository attributeRepository,
                                   DataSchemaRepository dataSchemaRepository, TaskDataRepository taskDataRepository,
                                   BlockWorkflowRepository blockWorkflowRepository, ForeignSendEventRepository foreignSendEventRepository,
                                   AgendaRepository agendaRepository, ForeignRecieveEventRepository foreignRecieveEventRepository, 
                                   ForeignAttributeMapRepository foreignAttributeMapRepository, FilterRepository filterRepository,
                                   ConnectionRequestRepository connectionRequestRepository, AuditMessageRepository auditMessageRepository,
                                   NotificationRepository notificationRepository, BpmsDbContext context, IMapper mapper)
        : base(filterRepository)
        {
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _flowRepository = flowRepository;
            _blockModelRepository = blockModelRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _systemAgendaRepository = systemAgendaRepository;
            _workflowRepository = workflowRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _attributeRepository = attributeRepository;
            _dataSchemaRepository = dataSchemaRepository;
            _taskDataRepository = taskDataRepository;
            _blockWorkflowRepository = blockWorkflowRepository;
            _foreignSendEventRepository = foreignSendEventRepository;
            _agendaRepository = agendaRepository;
            _foreignRecieveEventRepository = foreignRecieveEventRepository;
            _foreignAttributeMapRepository = foreignAttributeMapRepository;
            _connectionRequestRepository = connectionRequestRepository;
            _auditMessageRepository = auditMessageRepository;
            _notificationRepository = notificationRepository;
            _context = context;
            _mapper = mapper;
        }
        #pragma warning restore CS8618

        public async Task<IActionResult> RemoveReciever(BlockIdSenderIdDTO? dto)
        {
            _foreignRecieveEventRepository.Remove(new ForeignSignalRecieveEventEntity
            {
                ForeignBlockId = dto.BlockId,
                SenderId = dto.SenderId,
                SystemId = _system.Id
            });
            await _foreignRecieveEventRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> AddReciever(BlockIdSenderIdDTO? dto)
        {
            await _foreignRecieveEventRepository.Create(new ForeignSignalRecieveEventEntity
            {
                ForeignBlockId = dto.BlockId,
                SenderId = dto.SenderId,
                SystemId = _system.Id
            });
            await _foreignRecieveEventRepository.Save();

            return await CreateResult(await _blockModelRepository.MappedBareAttributes(dto.SenderId));
        }

        public async Task<IActionResult> Agendas()
        {
            return await CreateResult(await _agendaRepository.AgendasSystem(_system.Id));
        }

        public async Task<IActionResult> DeactivateSystem()
        {
            _system.State = SystemStateEnum.Deactivated;
            await NotificationHub.CreateSendNotifications(_notificationRepository, _system.Id, NotificationTypeEnum.DeactivatedSystem, 
                                                          _system.Name, null, await _userRepository.Admins());
            await _systemRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> ChangeEncryption(EncryptionLevelEnum encryption)
        {
            _system.ForeignEncryption = encryption;
            await _systemRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> ReactivateSystem(ConnectionRequestEntity? request)
        {
            _system.State = SystemStateEnum.Reactivated;
            await NotificationHub.CreateSendNotifications(_notificationRepository, _system.Id, NotificationTypeEnum.ReactivateSystem, 
                                                          _system.Name, null, await _userRepository.Admins());
            await _connectionRequestRepository.Create(request);
            await _connectionRequestRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> SenderBlocks(Guid poolId)
        {
            return await CreateResult(await _blockModelRepository.SenderBlocks(poolId));
        }

        public async Task<IActionResult> Pools(Guid modelId)
        {
            return await CreateResult(await _poolRepository.Pools(modelId));
        }

        public async Task<IActionResult> Models(Guid agendaId)
        {
            return await CreateResult(await _modelRepository.Models(agendaId));
        }

        public async Task<IActionResult> SenderInfo(Guid id)
        {
            return await CreateResult(await _blockModelRepository.SenderSignalInfo(id));
        }

        public async Task<IActionResult> ForeignRecieverInfo(Guid id)
        {
            return await CreateResult(await _blockModelRepository.ForeignRecieverInfo(id));
        }

        public async Task<IActionResult> ActivateSystem()
        {
            _system.State = SystemStateEnum.Active;
            _systemRepository.Update(_system);
            await NotificationHub.CreateSendNotifications(_notificationRepository, _system.Id, NotificationTypeEnum.ActivatedSystem, 
                                                          _system.Name, null, await _userRepository.Admins());

            await _systemRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> BlockActivity(List<BlockWorkflowActivityDTO>? blocks)
        {
            foreach (BlockWorkflowActivityDTO block in blocks)
            {
                if (await _blockWorkflowRepository.Any(block.WorkflowId, block.BlockModelId))
                {
                    BlockWorkflowEntity changedBlock = await _blockWorkflowRepository.Bare(block.WorkflowId, block.BlockModelId);
                    changedBlock.State = block.State;
                }
                else
                {
                    if (block.State == BlockWorkflowStateEnum.Active)
                    {
                        await _blockWorkflowRepository.Create(new BlockWorkflowEntity
                        {
                            State = BlockWorkflowStateEnum.Active,
                            BlockModelId = block.BlockModelId,
                            WorkflowId = block.WorkflowId
                        });
                    }
                }
            }

            await _blockWorkflowRepository.Save();
            return await CreateResult();
        }

        private async Task AssignMessage(MessageShare message, Dictionary<Guid, TaskDataEntity> taskData)
        {
            foreach (StringDataEntity data in message.Strings)
            {
                (taskData[data.AttributeId.Value] as IStringDataEntity).Value = data.Value;
            }

            foreach (NumberDataEntity data in message.Numbers)
            {
                (taskData[data.AttributeId.Value] as INumberDataEntity).Value = data.Value;
            }

            foreach (TextDataEntity data in message.Texts)
            {
                (taskData[data.AttributeId.Value] as ITextDataEntity).Value = data.Value;
            }

            foreach (DateDataEntity data in message.Dates)
            {
                (taskData[data.AttributeId.Value] as IDateDataEntity).Value = data.Value;
            }

            foreach (BoolDataEntity data in message.Bools)
            {
                (taskData[data.AttributeId.Value] as IBoolDataEntity).Value = data.Value;
            }

            foreach (SelectDataEntity data in message.Selects)
            {
                (taskData[data.AttributeId.Value] as ISelectDataEntity).Value = data.Value;
            }

            foreach (ArrayDataEntity data in message.Arrays)
            {
                (taskData[data.AttributeId.Value] as IArrayDataEntity).Type = data.Type;
            }

            foreach (FileDataEntity data in message.Files)
            {
                IFileDataEntity file = taskData[data.AttributeId.Value] as IFileDataEntity;
                file.MIMEType = data.MIMEType;
                file.FileName = data.FileName;
                await File.WriteAllBytesAsync(StaticData.FileStore + file.Id, data.Data);
            }
        }

        private async Task AssignForeignMessage(MessageShare? message, IEnumerable<IGrouping<Guid, TaskDataEntity>> taskDataGroup)
        {
            foreach (StringDataEntity data in message.Strings)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as IStringDataEntity).Value = data.Value;
                }
            }

            foreach (NumberDataEntity data in message.Numbers)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as INumberDataEntity).Value = data.Value;
                }
            }

            foreach (TextDataEntity data in message.Texts)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as ITextDataEntity).Value = data.Value;
                }
            }

            foreach (DateDataEntity data in message.Dates)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as IDateDataEntity).Value = data.Value;
                }
            }

            foreach (BoolDataEntity data in message.Bools)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as IBoolDataEntity).Value = data.Value;
                }
            }

            foreach (SelectDataEntity data in message.Selects)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as ISelectDataEntity).Value = data.Value;
                }
            }

            foreach (ArrayDataEntity data in message.Arrays)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    (taskData as IArrayDataEntity).Type = data.Type;
                }
            }

            foreach (FileDataEntity data in message.Files)
            {
                foreach (TaskDataEntity taskData in taskDataGroup.First(x => x.Key == data.AttributeId.Value))
                {
                    IFileDataEntity file = taskData as IFileDataEntity;
                    file.MIMEType = data.MIMEType;
                    file.FileName = data.FileName;
                    await File.WriteAllBytesAsync(StaticData.FileStore + file.Id, data.Data);
                }
            }
        }

        public async Task<IActionResult> Message(MessageShare? message)
        {
            Dictionary<Guid, TaskDataEntity> taskData;
            List<RecieveMessageEventWorkflowEntity> recieveEvents;
            if (message.WorkflowId != null)
            {
                taskData = await _taskDataRepository.OfRecieveEvent(message.WorkflowId.Value, message.BlockId);
                recieveEvents = await _blockWorkflowRepository.RecieveMessageEvents(message.WorkflowId.Value, message.BlockId);
            }
            else
            {
                taskData = await _taskDataRepository.OfRecieveEvent(message.BlockId);
                recieveEvents = await _blockWorkflowRepository.RecieveMessageEvents(message.BlockId);
            }

            await AssignMessage(message, taskData);
            
            WorkflowHelper workflowHelper = new WorkflowHelper(_context);
            foreach (RecieveMessageEventWorkflowEntity recieveEvent in recieveEvents)
            {
                recieveEvent.Delivered = true;
                if (recieveEvent.State == BlockWorkflowStateEnum.Active)
                {
                    recieveEvent.State = BlockWorkflowStateEnum.Solved;
                    await workflowHelper.StartNextTask(recieveEvent);
                }
                await workflowHelper.ShareActivity(recieveEvent.BlockModel.PoolId, recieveEvent.WorkflowId, recieveEvent.BlockModel.Pool.ModelId);
            }

            await _taskDataRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> ForeignMessage(MessageShare? message)
        {
            await AssignForeignMessage(message, await _taskDataRepository.OfForeignRecieveEvent(message.BlockId));

            WorkflowHelper workflowHelper = new WorkflowHelper(_context);
            foreach (RecieveSignalEventWorkflowEntity recieveEvent in await _blockWorkflowRepository.RecieveSignalEvents(message.BlockId))
            {
                recieveEvent.Delivered = true;
                if (recieveEvent.State == BlockWorkflowStateEnum.Active)
                {
                    recieveEvent.State = BlockWorkflowStateEnum.Solved;
                    await workflowHelper.StartNextTask(recieveEvent);
                }
                await workflowHelper.ShareActivity(recieveEvent.BlockModel.PoolId, recieveEvent.WorkflowId, recieveEvent.BlockModel.Pool.ModelId);
            }

            await _taskDataRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> RemoveRecieverAttribute(Guid id)
        {
            if (await _attributeRepository.Any(id))
            {
                _attributeRepository.Remove(new AttributeEntity
                {
                    Id = id,
                    MappedBlocks = await _attributeRepository.MappedBlocks(id)
                });
                await _attributeRepository.Save();
            }
            return await CreateResult();
        }

        public async Task<IActionResult> RemoveForeignRecieverAttribute(Guid id)
        {
            foreach (ForeignAttributeMapEntity attributeMap in await _foreignAttributeMapRepository.ForRemoval(id))
            {
                if (attributeMap.Attribute.Data.Count == 0)
                {
                    _attributeRepository.Remove(attributeMap.Attribute);
                }
                else
                {
                    attributeMap.Attribute.Disabled = true;
                }
            }
            await _attributeRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> CreateRecieverAttribute(AttributeEntity? attribute)
        {
            if (!await _attributeRepository.Any(attribute.Id))
            {
                await _attributeRepository.Create(attribute);
                await _attributeRepository.Save();
            }
            return await CreateResult();
        }

        public async Task<IActionResult> CreateForeignRecieverAttribute(AttributeEntity? attribute)
        {
            foreach (ForeignSendSignalEventEntity even in await _foreignSendEventRepository.BareReciever(attribute.BlockId))
            {
                if (!await _foreignAttributeMapRepository.Any(attribute.Id, even.Id))
                {
                    ForeignAttributeMapEntity map = new ForeignAttributeMapEntity
                    {
                        ForeignAttributeId = attribute.Id,
                        ForeignSendEventId = even.Id,
                        AttributeId = Guid.NewGuid()
                    };

                    attribute.Id = map.AttributeId;
                    attribute.BlockId = even.Reciever.Id;
                    await _attributeRepository.Create(_mapper.Map<AttributeEntity>(attribute));
                    await _foreignAttributeMapRepository.Create(map);
                }
            }

            await _foreignAttributeMapRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> UpdateForeignRecieverAttribute(AttributeEntity? attribute)
        {
            foreach (AttributeEntity attrib in await _foreignAttributeMapRepository.ForeignAttribs(attribute.Id))
            {
                attrib.Compulsory = attribute.Compulsory;
                attrib.Description = attribute.Description;
                attrib.Name = attribute.Name;
                attrib.Specification = attribute.Specification;
                attrib.Disabled = attribute.Disabled;
            }

            await _foreignAttributeMapRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> UpdateRecieverAttribute(AttributeEntity? attribute)
        {
            _attributeRepository.Update(attribute);
            await _attributeRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> ShareModel(ModelDetailShare? dto)
        {
            if (await _modelRepository.Any(dto.Id))
            {
                return await CreateResult();
            }

            ModelEntity model = _mapper.Map<ModelEntity>(dto);
            model.State = ModelStateEnum.Executable;
            XDocument svg = XDocument.Parse(dto.SVG, LoadOptions.PreserveWhitespace);
            
            AgendaTargetDTO targetAgenda = await _systemRepository.Agendas(dto.SenderURL);

            foreach (PoolShareDTO poolDTO in dto.Pools)
            {
                PoolEntity poolEntity = _mapper.Map<PoolEntity>(poolDTO);
                XElement element = svg.Descendants().First(x => x.Attribute("id")?.Value == poolEntity.Id.ToString());

                poolEntity.SystemId = await _systemRepository.IdFromUrl(poolDTO.SystemURL);
                if (poolEntity.SystemId == StaticData.ThisSystemId)
                {
                    element.Attribute("class").SetValue("djs-group bpmn-pool bpmn-this-sys");
                }
                else
                {
                    if (poolEntity.SystemId == Guid.Empty)
                    {
                        poolEntity.SystemId = null;
                    }

                    element.Attribute("class").SetValue("djs-group bpmn-pool");
                }

                await _poolRepository.Create(poolEntity);
            }

            model.SVG = svg.ToString(SaveOptions.DisableFormatting);
            model.AgendaId = targetAgenda.Id;
            await _modelRepository.Create(model);

            await _blockModelRepository.CreateRange(dto.EndEvents);
            await _blockModelRepository.CreateRange(dto.StartEvents);
            await _blockModelRepository.CreateRange(dto.UserTasks);
            await _blockModelRepository.CreateRange(dto.ServiceTasks);
            await _blockModelRepository.CreateRange(dto.ParallelGateways);
            await _blockModelRepository.CreateRange(dto.ExclusiveGateways);
            await _blockModelRepository.CreateRange(dto.SendMessageEvents);
            await _blockModelRepository.CreateRange(dto.RecieveMessageEvents);
            await _blockModelRepository.CreateRange(dto.SendSignalEvents);
            await _blockModelRepository.CreateRange(dto.RecieveSignalEvents);

            await _flowRepository.CreateRange(dto.Flows);

            await NotificationHub.CreateSendNotifications(_notificationRepository, model.Id, NotificationTypeEnum.NewModel, model.Name, null,
                                                          targetAgenda.AdministratorId);
           
            await _modelRepository.Save();
            return await CreateResult();
        }

        public async Task<IActionResult> IsModelRunable(WorkflowShare? dto)
        {
            ModelEntity model = await _modelRepository.StateAgendaPool(dto.ModelId, _system.Id);
            if (!await _workflowRepository.Any(dto.Workflow.Id))
            {
                dto.Workflow.AgendaId = model.AgendaId.Value;
                dto.Workflow.AdministratorId = null;
                await _workflowRepository.Create(dto.Workflow);
            }

            model.Pools[0].StartedId = model.Pools[0].Blocks.First(x => x is IStartEventModelEntity).Id;
            await NotificationHub.CreateSendNotifications(_notificationRepository, model.Id, NotificationTypeEnum.ModelRun, model.Name, null,
                                                          model.Agenda.AdministratorId);
            await _workflowRepository.Save();

            if (model.State != ModelStateEnum.Waiting)
            {
                throw new Exception(); // TODO
            }

            return await CreateResult();
        }

        public async Task<IActionResult> RunModel(ModelIdWorkflowDTO? dto)
        {
            await new WorkflowHelper(_context).CreateWorkflow(dto.ModelId, dto.WorkflowId);
            await _context.SaveChangesAsync();
            return await CreateResult();
        }

        public async Task<IActionResult> CreateSystem(SystemEntity? system)
        {
            await _systemRepository.Create(system);
            await NotificationHub.CreateSendNotifications(_notificationRepository, system.Id, NotificationTypeEnum.NewSystem, "", null,
                                                          await _userRepository.Admins());
            
            await _systemAgendaRepository.Save();
            return await CreateResult();
        }

        public async Task<string> AuthorizeSystem(string auth, HttpRequest request, string? action, HttpResponse response)
        {
            _response = response;
            auth = auth["Bearer ".Length..];
            (Guid id, byte[] byteAuth) = SymetricCipherHelper.ExtractGuid(auth);
            _system = _systemRepository.Bare(action == "CreateSystem" ? StaticData.ThisSystemId : id);
            SystemAuthorizationDTO authSystem = await SymetricCipherHelper.AuthDecrypt<SystemAuthorizationDTO>(byteAuth, _system.Key);

            bool active = action != "ReactivateSystem" && action != "ActivateSystem" && action != "CreateSystem";
            if (_system.State != SystemStateEnum.Active && _system.State != SystemStateEnum.ThisSystem && active)
            {
                throw new UnauthorizedAccessException();
            }

            using StreamReader stream = new StreamReader(request.Body);
            string data = await stream.ReadToEndAsync();
            if (_system.Encryption == EncryptionLevelEnum.Encrypted || _system.ForeignEncryption == EncryptionLevelEnum.Encrypted)
            {
                data = await SymetricCipherHelper.DecryptMessage(data, authSystem.PayloadKey, authSystem.PayloadIV);
            }

            if (_system.Encryption >= EncryptionLevelEnum.Hash || _system.ForeignEncryption >= EncryptionLevelEnum.Hash)
            {
                if (!SymetricCipherHelper.ArraysMatch(authSystem.PayloadHash, SymetricCipherHelper.HashMessage(data, authSystem.MessageId)))
                {
                    throw new UnauthorizedAccessException();    
                }
            }

            if (_system.Encryption >= EncryptionLevelEnum.Audit)
            {
                await AuditMessageTextHelper.CreateAuditMessage(_auditMessageRepository, authSystem.MessageId, id, action, request.Method);
            }
            _messageId = authSystem.MessageId;

            return data;
        }

        private async Task<IActionResult> CreateResult(object? data = null)
        {
            AddressDTO address = new AddressDTO(_messageId);
            address.Key = _system.Key;
            address.SystemId = _system.Id;
            address.Encryption = _system.Encryption > _system.ForeignEncryption ? _system.Encryption : _system.ForeignEncryption;

            string body;
            if (data == null)
            {
                body = _messageId.ToString();
            }
            else
            {
                body = JsonConvert.SerializeObject(data);
            }

            if (address.Encryption >= EncryptionLevelEnum.Hash)
            {
                address.PayloadHash = SymetricCipherHelper.HashMessage(body, address.MessageId);
            }

            if (address.Encryption == EncryptionLevelEnum.Encrypted)
            {
                body = await SymetricCipherHelper.EncryptMessage(body, address);
            }

            _response.Headers.Authorization = $"Bearer {await SymetricCipherHelper.AuthEncrypt(address)}";

            return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes(body)), "application/json");
        }
    }
}
