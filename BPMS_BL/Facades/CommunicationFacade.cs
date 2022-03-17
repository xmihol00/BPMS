using System.Security.Cryptography;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_BL.Hubs;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

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
            _system = new SystemEntity();
        }

        public async Task<string> RemoveReciever(BlockIdSenderIdDTO? dto)
        {
            _foreignRecieveEventRepository.Remove(new ForeignRecieveEventEntity
            {
                ForeignBlockId = dto.BlockId,
                SenderId = dto.SenderId,
                SystemId = _system.Id
            });
            await _foreignRecieveEventRepository.Save();

            return "";
        }

        public async Task<List<AttributeEntity?>> AddReciever(BlockIdSenderIdDTO? dto)
        {
            // TODO check system access
            await _foreignRecieveEventRepository.Create(new ForeignRecieveEventEntity
            {
                ForeignBlockId = dto.BlockId,
                SenderId = dto.SenderId,
                SystemId = _system.Id
            });
            await _foreignRecieveEventRepository.Save();

            return await _blockModelRepository.MappedBareAttributes(dto.SenderId);
        }

        public Task<List<AgendaIdNameDTO>> Agendas()
        {
            return _agendaRepository.AgendasSystem(_system.Id);
        }

        public async Task<string> DeactivateSystem()
        {
            _system.State = SystemStateEnum.Deactivated;
            await NotificationHub.CreateSendNotifications(_notificationRepository, _system.Id, NotificationTypeEnum.DeactivatedSystem, 
                                                          _system.Name, await _userRepository.Admins());
            await _systemRepository.Save();
            return "";
        }

        public async Task<string> ReactivateSystem(ConnectionRequestEntity? request)
        {
            _system.State = SystemStateEnum.Reactivated;
            await NotificationHub.CreateSendNotifications(_notificationRepository, _system.Id, NotificationTypeEnum.ReactivateSystem, 
                                                          _system.Name, await _userRepository.Admins());
            await _connectionRequestRepository.Create(request);
            await _connectionRequestRepository.Save();

            return "";
        }

        public Task<List<BlockIdNameDTO>> SenderBlocks(Guid poolId)
        {
            return _blockModelRepository.SenderBlocks(poolId);
        }

        public Task<List<PoolIdNameDTO>> Pools(Guid modelId)
        {
            return _poolRepository.Pools(modelId);
        }

        public Task<List<ModelIdNameDTO>> Models(Guid agendaId)
        {
            return _modelRepository.Models(agendaId);
        }

        public Task<SenderRecieverConfigDTO> SenderInfo(Guid id)
        {
            return _blockModelRepository.SenderInfo(id);
        }

        public Task<SenderRecieverConfigDTO> ForeignRecieverInfo(Guid id)
        {
            return _blockModelRepository.ForeignRecieverInfo(id);
        }

        public async Task<string> ActivateSystem()
        {
            _system.State = SystemStateEnum.Active;
            _systemRepository.Update(_system);
            await NotificationHub.CreateSendNotifications(_notificationRepository, _system.Id, NotificationTypeEnum.ActivatedSystem, 
                                                          _system.Name, await _userRepository.Admins());

            await _systemRepository.Save();

            return "";
        }

        public async Task<string> BlockActivity(List<BlockWorkflowActivityDTO>? blocks)
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
            return "";
        }
        private async Task AssignMessage(MessageShare message, Dictionary<Guid, TaskDataEntity> taskData, 
                                         List<RecieveEventWorkflowEntity> recieveEvents)
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

            foreach (RecieveEventWorkflowEntity recieveEvent in recieveEvents)
            {
                WorkflowHelper workflowHelper = new WorkflowHelper(_context);
                recieveEvent.Delivered = true;
                if (recieveEvent.State == BlockWorkflowStateEnum.Active)
                {
                    recieveEvent.State = BlockWorkflowStateEnum.Solved;
                    await workflowHelper.StartNextTask(recieveEvent);
                    //await _taskDataRepository.Save(); // TODO
                }
                await workflowHelper.ShareActivity(recieveEvent.BlockModel.PoolId, recieveEvent.WorkflowId, recieveEvent.BlockModel.Pool.ModelId);
            }
        }

        public async Task<string> Message(MessageShare? message)
        {
            Dictionary<Guid, TaskDataEntity> taskData;
            List<RecieveEventWorkflowEntity> recieveEvents;
            if (message.WorkflowId != null)
            {
                taskData = await _taskDataRepository.OfRecieveEvent(message.WorkflowId.Value, message.BlockId);
                recieveEvents = await _blockWorkflowRepository.RecieveEvents(message.WorkflowId.Value, message.BlockId);
            }
            else
            {
                taskData = await _taskDataRepository.OfRecieveEvent(message.BlockId);
                recieveEvents = await _blockWorkflowRepository.RecieveEvents(message.BlockId);
            }

            await AssignMessage(message, taskData, recieveEvents);
            await _taskDataRepository.Save();

            return "";
        }

        public async Task<string> ForeignMessage(MessageShare? message)
        {
            await AssignMessage(message, await _taskDataRepository.OfForeignRecieveEvent(message.BlockId), 
                                await _blockWorkflowRepository.RecieveEvents(message.BlockId));
            await _taskDataRepository.Save();

            return "";
        }

        public async Task<string> RemoveRecieverAttribute(Guid id)
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

            return "";
        }

        public async Task<string> RemoveForeignRecieverAttribute(Guid id)
        {
            foreach (AttributeEntity attribute in await _foreignAttributeMapRepository.ForRemoval(id))
            {
                _attributeRepository.Remove(attribute);
            }
            await _attributeRepository.Save();

            return "";
        }

        public async Task<string> CreateRecieverAttribute(AttributeEntity? attribute)
        {
            if (!await _attributeRepository.Any(attribute.Id))
            {
                await _attributeRepository.Create(attribute);
                await _attributeRepository.Save();
            }

            return "";
        }

        public async Task<string> CreateForeignRecieverAttribute(AttributeEntity? attribute)
        {
            
            foreach (ForeignSendEventEntity even in await _foreignSendEventRepository.BareReciever(attribute.BlockId))
            {
                if (!await _foreignAttributeMapRepository.Any(attribute.Id))
                {
                    ForeignAttributeMapEntity map = new ForeignAttributeMapEntity
                    {
                        ForeignAttributeId = attribute.Id,
                        ForeignSendEventId = even.Id,
                        AttributeId = Guid.NewGuid()
                    };

                    attribute.Id = map.AttributeId;
                    attribute.BlockId = even.Reciever.Id;
                    _attributeRepository.Update(_mapper.Map<AttributeEntity>(attribute));
                    _foreignAttributeMapRepository.Update(map);
                }
            }

            await _foreignAttributeMapRepository.Save();
            return "";
        }

        public async Task<string> ShareModel(ModelDetailShare? dto)
        {
            if (await _modelRepository.Any(dto.Id))
            {
                return "";
            }

            ModelEntity model = _mapper.Map<ModelEntity>(dto);
            model.State = ModelStateEnum.Executable;
            XDocument svg = XDocument.Parse(dto.SVG, LoadOptions.PreserveWhitespace);
            
            AgendaTargetDTO targetAgenda = await _systemRepository.Agendas(dto.SenderURL);

            foreach (PoolShareDTO poolDTO in dto.Pools)
            {
                PoolEntity poolEntity = _mapper.Map<PoolEntity>(poolDTO);
                XElement element = svg.Descendants().First(x => x.Attribute("id")?.Value == poolEntity.Id.ToString());

                if (poolDTO.SystemURL == StaticData.ThisSystemURL)
                {
                    poolEntity.SystemId = StaticData.ThisSystemId;
                    element.Attribute("class").SetValue("djs-group bpmn-pool bpmn-this-sys");
                }
                else
                {
                    poolEntity.SystemId = await _systemRepository.IdFromUrl(poolDTO.SystemURL);
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
            await _blockModelRepository.CreateRange(dto.SendEvents);
            await _blockModelRepository.CreateRange(dto.RecieveEvents);

            await _flowRepository.CreateRange(dto.Flows);

            await NotificationHub.CreateSendNotifications(_notificationRepository, model.Id, NotificationTypeEnum.NewModel, model.Name,
                                                          targetAgenda.AdministratorId);
           
            await _modelRepository.Save();

            return "";
        }

        public async Task<string> IsModelRunable(WorkflowShare? dto)
        {
            ModelEntity model = await _modelRepository.StateAgendaPool(dto.ModelId, _system.Id);
            if (!await _workflowRepository.Any(dto.Workflow.Id))
            {
                dto.Workflow.AgendaId = model.AgendaId.Value;
                dto.Workflow.AdministratorId = null;
                await _workflowRepository.Create(dto.Workflow);
            }

            model.Pools[0].StartedId = model.Pools[0].Blocks.First(x => x is IStartEventModelEntity).Id;
            await NotificationHub.CreateSendNotifications(_notificationRepository, model.Id, NotificationTypeEnum.ModelRun, model.Name,
                                                          model.Agenda.AdministratorId);
            await _workflowRepository.Save();

            if (model.State != ModelStateEnum.Waiting)
            {
                throw new Exception(); // TODO
            }

            return "";
        }

        public async Task<string> RunModel(ModelIdWorkflowDTO? dto)
        {
            await new WorkflowHelper(_context).CreateWorkflow(dto.ModelId, dto.WorkflowId);
            await _context.SaveChangesAsync();

            return "";   
        }

        public async Task<string> CreateSystem(SystemEntity? system)
        {
            await _systemRepository.Create(system);
            await NotificationHub.CreateSendNotifications(_notificationRepository, system.Id, NotificationTypeEnum.NewSystem, "",
                                                          await _userRepository.Admins());
            
            await _systemAgendaRepository.Save();
            return "";
        }

        public async Task<string> AuthorizeSystem(string auth, HttpRequest request, string? action)
        {
            auth = auth["Bearer ".Length..];
            (Guid id, auth) = SymetricCipherHelper.ExtractGuid(auth);
            _system = _systemRepository.Bare(id);
            SystemAuthorizationDTO authSystem = await SymetricCipherHelper.AuthDecrypt<SystemAuthorizationDTO>(auth, _system.Key);

            using StreamReader stream = new StreamReader(request.Body);
            string data = await stream.ReadToEndAsync();
            if (authSystem.PayloadKey != null)
            {
                data = await SymetricCipherHelper.Decrypt(data, authSystem.PayloadKey, authSystem.PayloadIV);
            }

            if (authSystem.PayloadHash != null)
            {
                if (authSystem.PayloadHash != new Rfc2898DeriveBytes(data, authSystem.MessageId.ToByteArray(), 1000).GetBytes(32))
                {
                    throw new UnauthorizedAccessException();    
                }
            }

            bool active = action != "ReactivateSystem" && action != "ActivateSystem" && action != "CreateSystem";
            if ((_system.State != SystemStateEnum.Active && _system.State != SystemStateEnum.ThisSystem && active) || authSystem.URL != _system.URL)
            {
                throw new UnauthorizedAccessException();
            }

            await AuditMessageTextHelper.CreateAuditMessage(_auditMessageRepository, authSystem.MessageId, id, action, request.Method);

            return data;
        }
    }
}
