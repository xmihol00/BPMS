
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Account;
using BPMS_DTOs.Attribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Pool;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMS_BL.Helpers
{
    public class WorkflowHelper
    {
        private readonly ModelRepository _modelRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly AttributeRepository _attributeRepository;
        private readonly DataSchemaRepository _dataSchemaRepository;
        private readonly BlockWorkflowRepository _taskRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly PoolRepository _poolRepository;
        private Dictionary<Guid, BlockWorkflowEntity> _createdUserTasks = new Dictionary<Guid, BlockWorkflowEntity>();
        private Dictionary<(Guid, Guid), TaskDataEntity> _createdServiceData = new Dictionary<(Guid, Guid), TaskDataEntity>();
        private Dictionary<Guid, TaskDataEntity> _createdTaskData = new Dictionary<Guid, TaskDataEntity>();

        #pragma warning disable CS8618
        public WorkflowHelper(BpmsDbContext context)
        {
            _modelRepository = new ModelRepository(context);
            _workflowRepository = new WorkflowRepository(context);
            _agendaRoleRepository = new AgendaRoleRepository(context);
            _attributeRepository = new AttributeRepository(context);
            _dataSchemaRepository = new DataSchemaRepository(context);
            _taskRepository = new BlockWorkflowRepository(context);
            _taskDataRepository = new TaskDataRepository(context);
            _blockModelRepository = new BlockModelRepository(context);
            _serviceRepository = new ServiceRepository(context);
            _poolRepository = new PoolRepository(context);
        }
        #pragma warning restore CS8618

        public async Task CreateWorkflow(Guid modelId, WorkflowEntity workflow)
        {
            ModelEntity model = await _modelRepository.DetailToCreateWF(modelId);
            model.State = ModelStateEnum.Executable;
            BlockModelEntity startEvent = model.Pools.First(x => x.SystemId == StaticData.ThisSystemId)
                                               .Blocks.First(x => x is IStartEventModelEntity);

            List<BlockWorkflowEntity> blocks = await CreateBlocks(startEvent);

            workflow.State = WorkflowStateEnum.Active;
            workflow.Blocks = blocks;

            blocks[0].SolvedDate = DateTime.Now;
            await ExecuteFirstBlock(startEvent.OutFlows[0].InBlock, blocks[1], workflow);
        }

        public async Task CreateWorkflow(Guid modelId, Guid workflowId)
        {
            await CreateWorkflow(modelId, await _workflowRepository.Waiting(workflowId));
        }

        private async Task<List<BlockWorkflowEntity>> CreateBlocks(BlockModelEntity blockModel)
        {
            List<BlockWorkflowEntity> blocks = new List<BlockWorkflowEntity>();
            blocks.Add(await CreateBlock(blockModel));
            foreach (FlowEntity outFlows in blockModel.OutFlows)
            {
                blocks.AddRange(await CreateBlocks(outFlows.InBlock));
            }

            return blocks;
        }

        private async Task<BlockWorkflowEntity> CreateBlock(BlockModelEntity blockModel)
        {
            BlockWorkflowEntity blockWorkflow;
            switch (blockModel)
            {
                case IUserTaskModelEntity:
                    blockWorkflow = CreateUserTask(blockModel);
                    break;

                case IServiceTaskModelEntity serviceTask:
                    blockWorkflow = new ServiceTaskWorkflowEntity()
                    {
                        UserId = null
                    };

                    IServiceTaskWorkflowEntity sTask = blockWorkflow as IServiceTaskWorkflowEntity;
                    sTask.OutputData = CrateServiceTaskData(await _dataSchemaRepository.AllWithMaps(serviceTask.ServiceId), serviceTask.Id);
                    break;

                case IEndEventModelEntity:
                    blockWorkflow = new EndEventWorkflowEntity();
                    break;

                case IStartEventModelEntity:
                    blockWorkflow = new StartEventWorkflowEntity();
                    break;

                case ISendEventModelEntity:
                    blockWorkflow = CreateSendEvent(blockModel);
                    break;

                case IRecieveEventModelEntity:
                    blockWorkflow = CreateRecieveEvent(blockModel);
                    break;

                default:
                    blockWorkflow = new BlockWorkflowEntity();
                    break;
            }
            blockWorkflow.State = BlockWorkflowStateEnum.Inactive;
            blockWorkflow.BlockModelId = blockModel.Id;

            return blockWorkflow;
        }

        private BlockWorkflowEntity CreateRecieveEvent(BlockModelEntity blockModel)
        {
            IRecieveEventModelEntity recieveEvent = blockModel as IRecieveEventModelEntity;
            RecieveEventWorkflowEntity blockWorkflow = new RecieveEventWorkflowEntity();
            blockWorkflow.OutputData = CrateUserTaskData(blockModel.Attributes);
            blockWorkflow.Delivered = recieveEvent.SenderId == null && recieveEvent.ForeignSenderId == null;
            return blockWorkflow;
        }

        private BlockWorkflowEntity CreateSendEvent(BlockModelEntity blockModel)
        {
            BlockWorkflowEntity blockWorkflow = new SendEventWorkflowEntity();

            foreach (AttributeMapEntity mappedAttribs in blockModel.MappedAttributes)
            {
                TaskDataEntity? taskData = _createdTaskData.GetValueOrDefault(mappedAttribs.AttributeId);
                if (taskData != null)
                {
                    blockWorkflow.InputData.Add(new TaskDataMapEntity
                    {
                        Task = blockWorkflow,
                        TaskData = taskData
                    });
                }
            }

            return blockWorkflow;
        }

        private BlockWorkflowEntity CreateUserTask(BlockModelEntity blockModel)
        {
            BlockWorkflowEntity blockWorkflow = new UserTaskWorkflowEntity()
            {
                UserId = null,
                Priority = TaskPriorityEnum.Medium
            };
            IUserTaskWorkflowEntity uTask = blockWorkflow as IUserTaskWorkflowEntity;

            _createdUserTasks[blockModel.Id] = blockWorkflow;
            foreach (BlockModelDataSchemaEntity schema in blockModel.DataSchemas)
            {
                TaskDataEntity? taskData = _createdServiceData.GetValueOrDefault((schema.DataSchemaId, schema.ServiceTaskId));
                if (taskData != null)
                {
                    blockWorkflow.InputData.Add(new TaskDataMapEntity
                    {
                        Task = blockWorkflow,
                        TaskData = taskData
                    });
                }
            }

            foreach (AttributeMapEntity mappedAttribs in blockModel.MappedAttributes)
            {
                TaskDataEntity? taskData = _createdTaskData.GetValueOrDefault(mappedAttribs.AttributeId);
                if (taskData != null)
                {
                    blockWorkflow.InputData.Add(new TaskDataMapEntity
                    {
                        Task = blockWorkflow,
                        TaskData = taskData
                    });
                }
            }

            uTask.OutputData = CrateUserTaskData(blockModel.Attributes);
            return blockWorkflow;
        }

        private List<TaskDataEntity> CrateServiceTaskData(List<DataSchemaEntity> dataSchemas, Guid serviceTaskId)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(DataSchemaEntity attrib in dataSchemas)
            {
                TaskDataEntity taskData = CreateServiceTaskData(attrib.Type);
                taskData.SchemaId = attrib.Id;
                _createdServiceData[(attrib.Id, serviceTaskId)] = taskData;
                data.Add(taskData);

                if (attrib.Direction == DirectionEnum.Input)
                {
                    foreach (BlockModelDataSchemaEntity mappedAttrib in attrib.Blocks.Where(x => x.ServiceTaskId == serviceTaskId))
                    {
                        BlockWorkflowEntity? blockWorkflow = _createdUserTasks.GetValueOrDefault(mappedAttrib.BlockId);
                        if (blockWorkflow != null)
                        {
                            blockWorkflow.InputData.Add(new TaskDataMapEntity
                            {
                                TaskData = taskData,
                                Task = blockWorkflow
                            });
                        }
                    }
                }
            }

            return data;
        }

        private TaskDataEntity CreateServiceTaskData(DataTypeEnum type)
        {
            switch (type)
            {
                case DataTypeEnum.String:
                    return new StringDataEntity();

                case DataTypeEnum.Number:
                    return new NumberDataEntity();
                
                case DataTypeEnum.Bool:
                    return new BoolDataEntity();
                                
                default:
                    return new TaskDataEntity();
            }
        }

        private List<TaskDataEntity> CrateUserTaskData(List<AttributeEntity> attributes)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(AttributeEntity attribute in attributes)
            {
                TaskDataEntity taskData = CreateUserTaskData(attribute.Type);
                taskData.AttributeId = attribute.Id;
                data.Add(taskData);
                _createdTaskData[attribute.Id] = taskData;
            }

            return data;
        }

        private TaskDataEntity CreateUserTaskData(AttributeTypeEnum type)
        {
            switch (type)
            {
                case AttributeTypeEnum.String:
                    return new StringDataEntity();

                case AttributeTypeEnum.Number:
                    return new NumberDataEntity();
                
                case AttributeTypeEnum.Text:
                    return new TextDataEntity();

                case AttributeTypeEnum.YesNo:
                    return new BoolDataEntity();
                
                case AttributeTypeEnum.File:
                    return new FileDataEntity();
                
                case AttributeTypeEnum.Select:
                    return new SelectDataEntity();
                
                case AttributeTypeEnum.Date:
                    return new DateDataEntity();
                
                default:
                    return new TaskDataEntity();
            }
        }

        private async Task ExecuteFirstBlock(BlockModelEntity blockModel, BlockWorkflowEntity blockWorkflow, WorkflowEntity workflow)
        {
            blockWorkflow.State = BlockWorkflowStateEnum.Active;
            if (blockWorkflow is IUserTaskWorkflowEntity)
            {
                IUserTaskModelEntity taskModel = blockModel as IUserTaskModelEntity;
                IUserTaskWorkflowEntity taskWorkflow = blockWorkflow as IUserTaskWorkflowEntity;
                taskWorkflow.UserId = await _agendaRoleRepository.LeastBussyUser(taskModel.RoleId ?? Guid.Empty) ?? workflow.AdministratorId;
                taskWorkflow.SolveDate = DateTime.Now.AddDays(taskModel.Difficulty.TotalDays);
            }
            else if (blockWorkflow is IUserTaskWorkflowEntity)
            {
                IServiceTaskModelEntity serviceModel = blockModel as IServiceTaskModelEntity;
                (blockWorkflow as IServiceTaskWorkflowEntity).UserId =
                        await _agendaRoleRepository.LeastBussyUser(serviceModel.RoleId ?? Guid.Empty) ?? workflow.AdministratorId;
            }

            await ShareActivity(blockWorkflow, workflow.Id, blockModel.Pool.ModelId);
        }

        public async Task StartNextTask(BlockWorkflowEntity solvedTask)
        {
            foreach (BlockWorkflowEntity task in await _taskRepository.NextBlocks(solvedTask.Id, solvedTask.WorkflowId))
            {
                switch (task)
                {
                    case IUserTaskWorkflowEntity:
                        await NextUserTask(task);
                        break;

                    case IServiceTaskWorkflowEntity:
                        await NextServiceTask(task);
                        break;
                    
                    case IEndEventWorkflowEntity:
                        await FinishWorkflow(task);
                        break;
                    
                    case ISendEventWorkflowEntity:
                        await SendData(task);
                        break;
                    
                    case IRecieveEventWorkflowEntity:
                        await RecieveData(task);
                        break;
                }
            }
        }

        private async Task RecieveData(BlockWorkflowEntity task)
        {
            if ((task as IRecieveEventWorkflowEntity).Delivered)
            {
                await StartNextTask(task);
            }
            else
            {
                task.State = BlockWorkflowStateEnum.Active;
            }
        }

        private async Task SendData(BlockWorkflowEntity task)
        {
            MessageShare dto = new MessageShare();

            foreach (var group in (await _taskDataRepository.MappedSendEventData(task.Id)).GroupBy(x => x.GetType()))
            {
                switch (group.First())
                {
                    case IStringDataEntity:
                        dto.Strings = group.Cast<StringDataEntity>();
                        break;
                    
                    case INumberDataEntity:
                        dto.Numbers = group.Cast<NumberDataEntity>();
                        break;
                    
                    case ITextDataEntity:
                        dto.Texts = group.Cast<TextDataEntity>();
                        break;
                    
                    case ISelectDataEntity:
                        dto.Selects = group.Cast<SelectDataEntity>();
                        break;
                    
                    case IFileDataEntity:
                        dto.Files = group.Cast<FileDataEntity>();
                        break;

                    case IBoolDataEntity:
                        dto.Bools = group.Cast<BoolDataEntity>();
                        break;
                    
                    case IArrayDataEntity:
                        dto.Arrays = group.Cast<ArrayDataEntity>();
                        break;
                    
                    case IDateDataEntity:
                        dto.Dates = group.Cast<DateDataEntity>();
                        break;
                }
            }

            bool recieved = true;
            foreach (BlockAddressDTO address in await _blockModelRepository.RecieverAddresses(task.BlockModelId))
            {
                if (await _blockModelRepository.IsInModel(task.BlockModelId, address.ModelId))
                {
                    dto.WorkflowId = task.WorkflowId;
                }
                else
                {
                    dto.WorkflowId = null;
                }
                dto.BlockId = address.BlockId;

                recieved &= await CommunicationHelper.Message(address.DestinationURL, 
                                                              SymetricCipherHelper.JsonEncrypt(address),
                                                              JsonConvert.SerializeObject(dto));
            }

            foreach (SenderRecieverAddressDTO address in await _blockModelRepository.ForeignRecieversAddresses(task.BlockModelId))
            {
                dto.BlockId = address.ForeignBlockId;
                recieved &= await CommunicationHelper.ForeignMessage(address.DestinationURL, 
                                                                     SymetricCipherHelper.JsonEncrypt(address),
                                                                     JsonConvert.SerializeObject(dto));
            }

            if (recieved)
            {
                task.State = BlockWorkflowStateEnum.Inactive;
                await StartNextTask(task);
            }
            else
            {
                task.State = BlockWorkflowStateEnum.Active;
            }
        }

        private async Task FinishWorkflow(BlockWorkflowEntity task)
        {
            WorkflowEntity workflow = await _workflowRepository.Bare(task.WorkflowId);
            workflow.End = DateTime.Now;
            workflow.State = WorkflowStateEnum.Finished;
        }

        private async Task NextServiceTask(BlockWorkflowEntity task)
        {
            IServiceTaskWorkflowEntity serviceTask = task as IServiceTaskWorkflowEntity;
            ServiceTaskModelEntity serviceTaskModel = await _blockModelRepository.ServiceTaskForSolve(serviceTask.BlockModelId);

            try
            {
                ServiceRequestDTO service = await _serviceRepository.ForRequest(serviceTaskModel.ServiceId.Value);
                service.Nodes = await CreateRequestTree(serviceTaskModel.ServiceId.Value, serviceTask.Id);
                ServiceCallResultDTO result = await new WebServiceHelper(service).SendRequest();
                await MapRequestResult(result, serviceTask.Id);

                await StartNextTask(task);
                serviceTask.State = BlockWorkflowStateEnum.Inactive;
            }
            catch
            {
                serviceTask.State = BlockWorkflowStateEnum.Active;
                serviceTask.UserId = await _agendaRoleRepository.LeastBussyUser(serviceTaskModel.RoleId ?? Guid.Empty) ??
                                                                                serviceTask.Workflow.AdministratorId;
            }
        }

        private async Task MapRequestResult(ServiceCallResultDTO result, Guid serviceTaskId)
        {
            List<TaskDataEntity> data = await _taskDataRepository.OutputServiceTaskData(serviceTaskId);

            switch (result.Serialization)
            {
                case SerializationEnum.JSON:
                    await MapRequestResultJson(JObject.Parse(result.RecievedData), data);
                    break;
                
                case SerializationEnum.XML:
                    throw new NotImplementedException();

                case SerializationEnum.URL:
                    throw new NotImplementedException();
                
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task MapRequestResultJson(IEnumerable<JToken> tokens, IEnumerable<TaskDataEntity> data)
        {
            TaskDataEntity currentData;
            foreach (JProperty property in tokens)
            {
                string name = property.Name;
                currentData = data.FirstOrDefault(x => x.Schema.Alias == name);
                if (currentData == null)
                {
                    continue;
                }

                switch (property.Value.Type)
                {
                    case JTokenType.Object:
                        await MapRequestResultJson(property.Value.Children(), data.Where(x => x.Schema.ParentId == currentData.Schema.Id));
                        continue;
                    
                    case JTokenType.String:
                        IStringDataEntity stringData = currentData as IStringDataEntity;
                        stringData.Value = ((string?)property.Value);
                        break;
                    
                    case JTokenType.Float:
                    case JTokenType.Integer:
                        INumberDataEntity numberData = currentData as INumberDataEntity;
                        numberData.Value = ((double?)property.Value);
                        break;

                    case JTokenType.Boolean:
                        IBoolDataEntity boolData = currentData as IBoolDataEntity;
                        boolData.Value = ((bool?)property.Value);
                        break;
                    
                    case JTokenType.Array:
                        // TODO
                        continue;
                }
            }
        }

        private async Task<IEnumerable<DataSchemaDataDTO>> CreateRequestTree(Guid serviceId, Guid serviceTaskId)
        {
            Dictionary<Guid, TaskDataEntity> data = await _taskDataRepository.InputServiceTaskData(serviceTaskId);
            IEnumerable<DataSchemaDataDTO> nodes = await _dataSchemaRepository.DataSchemaToSend(serviceId);
            foreach (DataSchemaDataDTO node in nodes)
            {
                if (node.StaticData == null)
                {
                    node.Data = StringDataOfTask(data.GetValueOrDefault(node.Id));    
                }
                else
                {
                    node.Data = node.StaticData;
                }

                if (node.Data == null && node.Compulsory && node.Type != DataTypeEnum.Object)
                {
                    throw new Exception(); // TODO
                }
            }

            return ServiceTreeHelper.CreateTree<DataSchemaDataDTO>(nodes);
        }

        private string? StringDataOfTask(TaskDataEntity? taskDataEntity)
        {
            if (taskDataEntity == null)
            {
                return null;
            }
            else
            {
                switch (taskDataEntity)
                {
                    case IStringDataEntity stringData:
                        return stringData.Value;
                    
                    case INumberDataEntity numberData:
                        return numberData.Value?.ToString();
                    
                    case IBoolDataEntity boolData:
                        return boolData.Value?.ToString();
                    
                    default:
                        return null;
                }
            }
        }

        private async Task NextUserTask(BlockWorkflowEntity task)
        {
            IUserTaskWorkflowEntity userTask = task as IUserTaskWorkflowEntity;
            userTask.State = BlockWorkflowStateEnum.Active;
            UserTaskModelEntity userTaskModel = await _blockModelRepository.UserTaskForSolve(userTask.BlockModelId);
            userTask.SolveDate = DateTime.Now.AddDays(userTaskModel.Difficulty.TotalDays);
            userTask.UserId = await _agendaRoleRepository.LeastBussyUser(userTaskModel.RoleId ?? Guid.Empty) ??
                              userTask.Workflow.AdministratorId;
        }

        public async Task ShareActivity(Guid poolId, Guid workflowId, Guid modelId)
        {
            List<BlockWorkflowActivityDTO> activeBlocks = await _taskRepository.BlockActivity(poolId, workflowId);
            string message = JsonConvert.SerializeObject(activeBlocks);

            foreach (DstAddressDTO address in await _poolRepository.Addresses(modelId))
            {
                await CommunicationHelper.BlockActivity(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address), message);
            }
        }

        public async Task ShareActivity(BlockWorkflowEntity blockWorkflow, Guid worflowId, Guid modelId)
        {
            List<BlockWorkflowActivityDTO> activeBlocks = new List<BlockWorkflowActivityDTO>()
            {
                new BlockWorkflowActivityDTO()
                {
                    State = blockWorkflow.State,
                    BlockModelId = blockWorkflow.BlockModelId,
                    WorkflowId = worflowId
                }
            };
            string message = JsonConvert.SerializeObject(activeBlocks);

            foreach (DstAddressDTO address in await _poolRepository.Addresses(modelId))
            {
                await CommunicationHelper.BlockActivity(address.DestinationURL, SymetricCipherHelper.JsonEncrypt(address), message);
            }
        }
    }
}
