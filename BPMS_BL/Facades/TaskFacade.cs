using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace BPMS_BL.Facades
{
    public class TaskFacade
    {
        private readonly BlockWorkflowRepository _taskRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly ServiceDataSchemaRepository _serviceDataSchemaRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly IMapper _mapper;

        public TaskFacade(BlockWorkflowRepository taskRepository, TaskDataRepository taskDataRepository, 
                          BlockModelRepository blockModelRepository, AgendaRoleRepository agendaRoleRepository, 
                          ServiceDataSchemaRepository serviceDataSchemaRepository, ServiceRepository serviceRepository, 
                          WorkflowRepository workflowRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _taskDataRepository = taskDataRepository;
            _blockModelRepository = blockModelRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
            _serviceRepository = serviceRepository;
            _workflowRepository = workflowRepository;
            _mapper = mapper;
        }

        public async Task SaveData(IFormCollection data, bool save = true)
        {
            foreach (KeyValuePair<string, StringValues> valuePair in data.Skip(1))
            {
                TaskDataEntity task = await _taskDataRepository.Detail(Guid.Parse(valuePair.Key));
                switch (task)
                {
                    case IBoolDataEntity boolData:
                        if (String.IsNullOrEmpty(valuePair.Value))
                        {
                            boolData.Value = null;
                            break;
                        }
                        boolData.Value = Boolean.Parse(valuePair.Value);
                        break;
                    
                    case IArrayDataEntity arrayData:
                        break;
                    
                    case INumberDataEntity numberData:
                        if (String.IsNullOrEmpty(valuePair.Value))
                        {
                            numberData.Value = null;
                            break;
                        }
                        numberData.Value = Double.Parse(valuePair.Value);
                        break;
                    
                    case IStringDataEntity stringData:
                        stringData.Value = valuePair.Value;
                        break;

                    case ITextDataEntity textData:
                        textData.Value = valuePair.Value;
                        break;

                    case IFileDataEntity fileData:
                        // TODO
                        break;
                    
                    case ISelectDataEntity selectData:
                        selectData.Value = valuePair.Value;
                        break;
                    
                    case IDateDataEntity dateData:
                        dateData.Value = DateTime.Parse(valuePair.Value);
                        break;
                }
            }

            if (save)
            {
                await _taskDataRepository.Save();
            }
        }

        public async Task SolveUserTask(IFormCollection data)
        {
            await SaveData(data, false);
            BlockWorkflowEntity solvedTask = await _taskRepository.TaskForSolving(Guid.Parse(data["TaskId"]));

            await StartNextTask(solvedTask);

            solvedTask.Active = false;
            solvedTask.SolvedDate = DateTime.Now;

            await _taskRepository.Save();
        }

        private async Task StartNextTask(BlockWorkflowEntity solvedTask)
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
                    
                    case IEndEventModelEntity:
                        await FinishWorkflow(task);
                        break;
                }
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
                serviceTask.Active = false;
            }
            catch
            {
                serviceTask.Active = true;
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
            IEnumerable<DataSchemaDataDTO> nodes = await _serviceDataSchemaRepository.DataSchemaToSend(serviceId);
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
            userTask.Active = true;
            UserTaskModelEntity userTaskModel = await _blockModelRepository.UserTaskForSolve(userTask.BlockModelId);
            userTask.SolveDate = DateTime.Now.AddDays(userTaskModel.Difficulty.TotalDays);
            userTask.UserId = await _agendaRoleRepository.LeastBussyUser(userTaskModel.RoleId ?? Guid.Empty) ??
                              userTask.Workflow.AdministratorId;
        }

        public Task<object?> ServiceTaskDetail(Guid id, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskOverviewDTO> Overview(Guid userId)
        {
            return new TaskOverviewDTO()
            {
                Tasks = await _taskRepository.Overview(userId)
            };
        }

        public async Task<UserTaskDetailDTO> UserTaskDetail(Guid id, Guid userId)
        {
            UserTaskDetailDTO detail = await _taskRepository.UserDetail(id, userId);
            var entity = await _taskRepository.Detail(id);
            
            foreach (TaskDataEntity data in await _taskDataRepository.MappedUserTasks(id))
            {
                detail.InputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
            }

            foreach (TaskDataEntity data in await _taskDataRepository.OutputUserTasks(id))
            {
                detail.OutputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
            }

            foreach (TaskDataEntity data in await _taskDataRepository.MappedServiceTasks(id))
            {
                if (data.Schema.Direction == DirectionEnum.Input)
                {
                    detail.InputServiceData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
                else
                {
                    detail.OutputServiceData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
            }

            return detail;
        }
    }
}
