using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using BPMS_DTOs.User;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class CommunicationFacade
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
        private readonly BlockAttributeRepository _blockAttributeRepository;
        private readonly ServiceDataSchemaRepository _serviceDataSchemaRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockWorkflowRepository _blockWorkflowRepository;
        private readonly BpmsDbContext _context;
        private readonly IMapper _mapper;
        private SystemEntity _system;

        public CommunicationFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                                   BlockModelRepository blockModelRepository, PoolRepository poolRepository, SystemRepository systemRepository, 
                                   SystemAgendaRepository systemAgendaRepository, WorkflowRepository workflowRepository,
                                   AgendaRoleRepository agendaRoleRepository, BlockAttributeRepository blockAttributeRepository,
                                   ServiceDataSchemaRepository serviceDataSchemaRepository, TaskDataRepository taskDataRepository,
                                   BlockWorkflowRepository blockWorkflowRepository, BpmsDbContext context, IMapper mapper)
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
            _blockAttributeRepository = blockAttributeRepository;
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
            _taskDataRepository = taskDataRepository;
            _blockWorkflowRepository = blockWorkflowRepository;
            _context = context;
            _mapper = mapper;
            _system = new SystemEntity();
        }

        public Task<List<BlockIdNameDTO>> SenderBlocks(Guid poolId)
        {
            return _blockModelRepository.SenderBlocks(poolId);
        }

        public Task<List<PoolIdNameDTO>> Pools(Guid modelId)
        {
            return _poolRepository.Pools(modelId);
        }

        public Task<List<ModelIdNameDTO>> Models()
        {
            return _systemRepository.Models(_system.Id);
        }

        public Task<SenderRecieverConfigDTO> SenderInfo(Guid id)
        {
            return _blockModelRepository.SenderInfo(id);
        }

        public async Task<string> ActivateSystem()
        {
            _system.State = SystemStateEnum.Active;
            _systemRepository.Update(_system);
            await _systemRepository.Save();

            return "";
        }

        public async Task<string> BlockActivity(List<BlockWorkflowActivityDTO> blocks)
        {
            foreach (BlockWorkflowActivityDTO block in blocks)
            {
                if (await _blockWorkflowRepository.Any(block.WorkflowId, block.BlockModelId))
                {
                    BlockWorkflowEntity changedBlock = await _blockWorkflowRepository.Bare(block.WorkflowId, block.BlockModelId);
                    changedBlock.Active = block.Active;
                }
                else
                {
                    if (block.Active)
                    {
                        await _blockWorkflowRepository.Create(new BlockWorkflowEntity
                        {
                            Active = true,
                            BlockModelId = block.BlockModelId,
                            WorkflowId = block.WorkflowId
                        });
                    }
                }
            }

            await _blockWorkflowRepository.Save();
            return "";
        }

        public async Task<string> Message(MessageShare message)
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
                (taskData[data.AttributeId.Value] as IFileDataEntity).MIMEType = data.MIMEType;
                (taskData[data.AttributeId.Value] as IFileDataEntity).FileName = data.FileName;
            }

            foreach (RecieveEventWorkflowEntity recieveEvent in recieveEvents)
            {
                WorkflowHelper workflowHelper = new WorkflowHelper(_context);
                recieveEvent.Delivered = true;
                if (recieveEvent.Active)
                {
                    recieveEvent.Active = false;
                    await workflowHelper.StartNextTask(recieveEvent);
                    await _taskDataRepository.Save();
                }
                await workflowHelper.ShareActivity(recieveEvent.BlockModel.PoolId, recieveEvent.WorkflowId, recieveEvent.BlockModel.Pool.ModelId);
            }

            await _taskDataRepository.Save();

            return "";
        }

        public async Task<string> RemoveRecieverAttribute(Guid id)
        {
            _blockAttributeRepository.Remove(new BlockAttributeEntity
            {
                Id = id,
                MappedBlocks = await _blockAttributeRepository.MappedBlocks(id)
            });
            await _blockAttributeRepository.Save();

            return "";
        }

        public async Task<string> ToggleRecieverAttribute(BlockAttributeEntity attribute)
        {
            if (await _blockAttributeRepository.Any(attribute.Id))
            {
                _blockAttributeRepository.Remove(attribute);
            }
            else
            {
                await _blockAttributeRepository.Create(attribute);
            }

            await _blockAttributeRepository.Save();
            return "";
        }

        public async Task<string> ShareModel(ModelDetailShare dto)
        {
            if (await _modelRepository.Any(dto.Id))
            {
                return "";
            }

            ModelEntity model = _mapper.Map<ModelEntity>(dto);
            model.State = ModelStateEnum.Shared;
            XDocument svg = XDocument.Parse(dto.SVG, LoadOptions.PreserveWhitespace);
            
            List<Guid> targetAgendas = await _systemRepository.Agendas(dto.SenderURL);
            if (targetAgendas.Count == 0)
            {
                throw new Exception();
            }
            else if (targetAgendas.Count == 1)
            {
                model.AgendaId = targetAgendas.First();
            }
            else
            {
                model.AgendaId = null;
            }

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
           
            await _modelRepository.Save();

            return "";
        }

        public async Task<string> IsModelRunable(WorkflowShare dto)
        {
            ModelEntity model = await _modelRepository.StateAgendaId(dto.ModelId);
            if (!await _workflowRepository.Any(dto.Workflow.Id))
            {
                dto.Workflow.AgendaId = model.AgendaId.Value;
                dto.Workflow.AdministratorId = null;
                await _workflowRepository.Create(dto.Workflow);
                await _workflowRepository.Save();
            }

            if (model.State != ModelStateEnum.Waiting)
            {
                throw new NotImplementedException(); // TODO
            }
            return "";
        }

        public async Task<string> RunModel(ModelIdWorkflowDTO dto)
        {
            await new WorkflowHelper(_context).CreateWorkflow(dto.ModelId, dto.WorkflowId);
            await _context.SaveChangesAsync();
            
            return "";   
        }

        public async Task<string> CreateSystem(SystemEntity system)
        {
            await _systemRepository.Create(system);
            await _systemAgendaRepository.Save();
            return "";
        }

        public void AuthorizeSystem(string auth, string path)
        {
            auth = auth["Bearer ".Length..];
            (Guid id, auth) = SymetricCipherHelper.ExtractGuid(auth);
            _system = _systemRepository.Bare(id);
            SystemAuthorizationDTO authSystem = 
                SymetricCipherHelper.JsonDecrypt<SystemAuthorizationDTO>(auth, _system.Key);

            if (authSystem.URL != _system.URL)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
