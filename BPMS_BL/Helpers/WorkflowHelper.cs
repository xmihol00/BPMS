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
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;
using BPMS_BL.Hubs;
using System.Net;
using System.Xml.Linq;

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
        private readonly NotificationRepository _notificationRepository;
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
            _notificationRepository = new NotificationRepository(context);
        }
        #pragma warning restore CS8618

        public async Task CreateWorkflow(Guid modelId, WorkflowEntity workflow)
        {
            ModelEntity model = await _modelRepository.DetailToCreateWF(modelId);
            model.State = ModelStateEnum.Executable;
            BlockModelEntity startEvent = model.Pools.First(x => x.SystemId == StaticData.ThisSystemId)
                                               .Blocks.First(x => x is IStartEventModelEntity);

            workflow.State = WorkflowStateEnum.Active;
            workflow.Blocks = await CreateBlocks(startEvent);
            workflow.Blocks[0].SolvedDate = DateTime.Now;
            await _modelRepository.Save();

            await StartNextTask(workflow.Blocks[0]);
            await ShareActivity(startEvent.PoolId, workflow.Id, model.Id);
            await NotificationHub.CreateSendNotifications(_notificationRepository, workflow.Id, NotificationTypeEnum.NewWorkflow, 
                                                          workflow.Name, workflow.AdministratorId.Value);
        }

        public async Task CreateWorkflow(Guid modelId, Guid workflowId)
        {
            await CreateWorkflow(modelId, await _workflowRepository.Waiting(workflowId));
        }

        public async Task<string?> CallService(IServiceTaskWorkflowEntity serviceTask)
        {
            ServiceTaskModelEntity serviceTaskModel = await _blockModelRepository.ServiceTaskForSolve(serviceTask.BlockModelId);

            ServiceCallResultDTO? result = null;
            try
            {
                ServiceRequestDTO service = await _serviceRepository.ForRequest(serviceTaskModel.ServiceId.Value);
                service.Nodes = await CreateRequestTree(serviceTaskModel.ServiceId.Value, serviceTask.Id);
                result = await new WebServiceHelper(service).SendRequest();
            }
            catch
            {
                return result.RecievedData;
            }
            
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return result.RecievedData;
            }

            try
            {
                await MapRequestResult(result, serviceTask.Id, serviceTask.WorkflowId);
            }
            catch
            {
                return result.RecievedData;
            }

            return null;
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
            blockWorkflow.State = BlockWorkflowStateEnum.NotStarted;
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
                
                case DataTypeEnum.ArrayString:
                case DataTypeEnum.ArrayBool:
                case DataTypeEnum.ArrayNumber:
                //case DataTypeEnum.ArrayObject:
                //case DataTypeEnum.ArrayArray:
                    return new ArrayDataEntity();
                                
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
                        foreach (IFileDataEntity file in dto.Files)
                        {
                            if (file.FileName != null)
                            {
                                file.Data = await File.ReadAllBytesAsync(StaticData.FileStore + file.Id);
                            }
                        }
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
                task.State = BlockWorkflowStateEnum.NotStarted;
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
                serviceTask.FailedResponse = result.RecievedData;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
                await MapRequestResult(result, serviceTask.Id, serviceTask.WorkflowId);

                await StartNextTask(task);
                serviceTask.State = BlockWorkflowStateEnum.NotStarted;
                serviceTask.FailedResponse = null;
            }
            catch
            {
                serviceTask.State = BlockWorkflowStateEnum.Active;
                serviceTask.UserId = await _agendaRoleRepository.LeastBussyUser(serviceTaskModel.RoleId ?? Guid.Empty) ??
                                                                                serviceTask.Workflow.AdministratorId;
            }
        }

        private async Task MapRequestResult(ServiceCallResultDTO result, Guid serviceTaskId, Guid workflowId)
        {
            List<TaskDataEntity> data = await _taskDataRepository.OutputServiceTaskData(serviceTaskId);
            List<DataSchemaDataMap> mappedData = await _taskDataRepository.MappedServiceTaskData(serviceTaskId, workflowId);

            switch (result.Serialization)
            {
                case SerializationEnum.JSON:
                    MapRequestResultJson(JObject.Parse(result.RecievedData), data, mappedData);
                    break;
                
                case SerializationEnum.XMLMarks:
                    MapRequestResultXml(XDocument.Parse(result.RecievedData), data, mappedData);
                    break;

                case SerializationEnum.URL:
                    throw new NotImplementedException();
                
                default:
                    throw new NotImplementedException();
            }

            if (data.Any(x => !x.HasData()))
            {
                throw new Exception();
            }
        }

        private void MapRequestResultXml(XDocument xml, List<TaskDataEntity> data, List<DataSchemaDataMap> mappedData)
        {
            Guid? parentId = null;
            if (xml.Root.HasAttributes)
            {
                MapRequestResultXmlAttributes(xml.Root.Attributes(), data, mappedData, null);
            }

            if (xml.Root.HasElements)
            {
                MapRequestResultXmlNodes(xml.Root.Nodes(), data, mappedData, null);
            }

            if (!xml.Root.HasElements && !xml.Root.HasAttributes) 
            {
                (TaskDataEntity currentData, List<TaskDataEntity> mappedCurrent) = GetDataToMap(data, mappedData, xml.Root.Name.LocalName, parentId);
                if (currentData != null)
                {
                    MapRequestResultXmlValue(currentData, mappedCurrent, xml.Root.Value);
                }
            }
        }

        private void MapRequestResultXmlNodes(IEnumerable<XNode> nodes, IEnumerable<TaskDataEntity> data, IEnumerable<DataSchemaDataMap> mappedData, Guid? parentId)
        {
            foreach (XElement child in nodes)
            {
                (TaskDataEntity currentData, List<TaskDataEntity> mappedCurrent) = GetDataToMap(data, mappedData, child.Name.LocalName, parentId);
                if (currentData != null)
                {
                    if (child.HasAttributes)
                    {
                        MapRequestResultXmlAttributes(child.Attributes(), data, mappedData, currentData.SchemaId);
                    }

                    if (child.HasElements)
                    {
                        MapRequestResultXmlNodes(child.Nodes(), data, mappedData, currentData.SchemaId);
                    }
                    else if (!child.HasAttributes)
                    {
                        MapRequestResultXmlValue(currentData, mappedCurrent, child.Value);
                    }
                }
            }
        }

        private void MapRequestResultXmlAttributes(IEnumerable<XAttribute> attributes, IEnumerable<TaskDataEntity> data, 
                                                   IEnumerable<DataSchemaDataMap> mappedData, Guid? parentId)
        {
            foreach (XAttribute attribute in attributes)
            {
                (TaskDataEntity currentData, List<TaskDataEntity> mappedCurrent) = GetDataToMap(data, mappedData, attribute.Name.LocalName, parentId);
                if (currentData != null)
                {
                    MapRequestResultXmlValue(currentData, mappedCurrent, attribute.Value);
                }
            }
        }

        private void MapRequestResultXmlValue(TaskDataEntity currentData, List<TaskDataEntity> mappedCurrent, string value)
        {
            switch (currentData)
            {
                case IBoolDataEntity boolData:
                    boolData.Value = Boolean.Parse(value);
                    if (boolData.Value != null)
                    {
                        foreach (TaskDataEntity mapped in mappedCurrent)
                        {
                                (mapped as IBoolDataEntity).Value = boolData.Value;
                        }
                    }
                    break;
                
                case INumberDataEntity numberData:
                    numberData.Value = Double.Parse(value);
                    if (numberData.Value != null)
                    {
                        foreach (TaskDataEntity mapped in mappedCurrent)
                        {
                                (mapped as INumberDataEntity).Value = numberData.Value;
                        }
                    }
                    break;
                
                case IStringDataEntity stringData:
                    stringData.Value = value;
                    if (stringData.Value != null)
                    {
                        foreach (TaskDataEntity mapped in mappedCurrent)
                        {
                            (mapped as IStringDataEntity).Value = stringData.Value;
                        }
                    }
                    break;
            }
        }

        private void MapRequestResultJson(IEnumerable<JToken> tokens, IEnumerable<TaskDataEntity> data, 
                                                IEnumerable<DataSchemaDataMap> mappedData, Guid? parentId = null)
        {
            foreach (JProperty property in tokens)
            {
                (TaskDataEntity currentData, List<TaskDataEntity> mappedCurrent) = GetDataToMap(data, mappedData, property.Name, parentId);
                if (currentData != null)
                {
                    switch (property.Value.Type)
                    {
                        case JTokenType.Object:
                            MapRequestResultJson(property.Value.Children(),
                                                 data.Where(x => x.Schema.ParentId == currentData.SchemaId),
                                                 mappedData.Where(x => x.ParentId == currentData.SchemaId), currentData.SchemaId);
                            continue;

                        case JTokenType.String:
                            IStringDataEntity stringData = currentData as IStringDataEntity;
                            stringData.Value = ((string?)property.Value);
                            if (stringData.Value != null)
                            {
                                foreach (TaskDataEntity mapped in mappedCurrent)
                                {
                                    (mapped as IStringDataEntity).Value = stringData.Value;
                                }
                            }
                            break;

                        case JTokenType.Float:
                        case JTokenType.Integer:
                            INumberDataEntity numberData = currentData as INumberDataEntity;
                            numberData.Value = ((double?)property.Value);
                            if (numberData.Value != null)
                            {
                                foreach (TaskDataEntity mapped in mappedCurrent)
                                {
                                    (mapped as INumberDataEntity).Value = numberData.Value;
                                }
                            }
                            break;

                        case JTokenType.Boolean:
                            IBoolDataEntity boolData = currentData as IBoolDataEntity;
                            boolData.Value = ((bool?)property.Value);
                            if (boolData.Value != null)
                            {
                                foreach (TaskDataEntity mapped in mappedCurrent)
                                {
                                    (mapped as IBoolDataEntity).Value = boolData.Value;
                                }
                            }
                            break;

                        case JTokenType.Array:
                            // TODO
                            continue;
                    }
                }
            }
        }

        private (TaskDataEntity?, List<TaskDataEntity>) GetDataToMap(IEnumerable<TaskDataEntity> data, IEnumerable<DataSchemaDataMap> mappedData, string name, Guid? parentId = null)
        {
            return (data.FirstOrDefault(x => x.Schema.ParentId == parentId && (x.Schema.Alias == name || (x.Schema.Alias == null && x.Schema.Name == name))),
                    mappedData.FirstOrDefault(x => x.ParentId == parentId && (x.Alias == name || (x.Alias == null && x.Name == name)))?.Data ?? new List<TaskDataEntity>());
        }

        private async Task<IEnumerable<DataSchemaDataDTO>> CreateRequestTree(Guid serviceId, Guid serviceTaskId)
        {
            Dictionary<Guid, IGrouping<Guid, TaskDataEntity>> data = await _taskDataRepository.InputServiceTaskData(serviceTaskId);
            List<DataSchemaDataDTO> nodes = await _dataSchemaRepository.DataSchemaToSend(serviceId);
            List<DataSchemaDataDTO> additional = new List<DataSchemaDataDTO>();
            foreach (DataSchemaDataDTO node in nodes)
            {
                if (node.StaticData == null)
                {
                    if (node.Type > DataTypeEnum.Object)
                    {
                        TaskDataEntity array = data[node.Id].First(x => x is IArrayDataEntity);
                        foreach (TaskDataEntity arrayData in data[node.Id])
                        {
                            if (arrayData.Id != array.Id)
                            {
                                additional.Add(new DataSchemaDataDTO
                                {
                                    Data = StringDataOfTask(arrayData),
                                    ParentId = array.SchemaId,
                                    Type = arrayData.Schema.Type
                                });
                            }
                        }
                    }
                    else
                    {
                        node.Data = StringDataOfTask(data.GetValueOrDefault(node.Id)?.First());
                    }
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

            nodes.AddRange(additional);
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

            await NotificationHub.CreateSendNotifications(_notificationRepository, userTask.Id, NotificationTypeEnum.NewTask, 
                                                          userTaskModel.Name, userTask.UserId.Value);
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
