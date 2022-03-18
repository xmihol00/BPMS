using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Task.DataTypes;
using BPMS_DTOs.Task.IDataTypes;
using BPMS_DTOs.Service;
using BPMS_DAL.Interfaces.WorkflowBlocks;

namespace BPMS_BL.Facades
{
    public class TaskFacade : BaseFacade
    {
        private readonly BlockWorkflowRepository _taskRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly DataSchemaRepository _dataSchemaRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly PoolRepository _poolRepository;
        private readonly BlockWorkflowRepository _blockWorkflowRepository;
        private readonly TaskDataMapRepository _taskDataMapRepository;
        private readonly BpmsDbContext _context;
        private WorkflowHelper _worflowHelper { get; set; }
        private readonly IMapper _mapper;

        #pragma warning disable CS8618
        public TaskFacade(BlockWorkflowRepository taskRepository, TaskDataRepository taskDataRepository, 
                          BlockModelRepository blockModelRepository, AgendaRoleRepository agendaRoleRepository, 
                          DataSchemaRepository dataSchemaRepository, ServiceRepository serviceRepository, 
                          WorkflowRepository workflowRepository, PoolRepository poolRepository, FilterRepository filterRepository,
                          BlockWorkflowRepository blockWorkflowRepository, TaskDataMapRepository taskDataMapRepository, 
                          BpmsDbContext context, IMapper mapper)
        : base(filterRepository)
        {
            _taskRepository = taskRepository;
            _taskDataRepository = taskDataRepository;
            _blockModelRepository = blockModelRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _dataSchemaRepository = dataSchemaRepository;
            _serviceRepository = serviceRepository;
            _workflowRepository = workflowRepository;
            _poolRepository = poolRepository;
            _blockWorkflowRepository = blockWorkflowRepository;
            _taskDataMapRepository = taskDataMapRepository;
            _context = context;
            _mapper = mapper;
        }
#pragma warning restore CS8618

        public async Task<UserTaskDetailPartialDTO> SaveUserTask(IFormCollection data, IFormFileCollection files)
        {
            await SaveDataInternal(data, files);
            await _taskDataRepository.Save();

            return await UserTaskDetail(Guid.Parse(data["TaskId"]));
        }

        public async Task SolveTask(IFormCollection data, IFormFileCollection files, BlockWorkflowStateEnum state = BlockWorkflowStateEnum.Solved)
        {
            await SaveDataInternal(data, files);
            _worflowHelper = new WorkflowHelper(_context);

            BlockWorkflowEntity solvedTask = await _taskRepository.TaskForSolving(Guid.Parse(data["TaskId"]));
            await _worflowHelper.StartNextTask(solvedTask);

            solvedTask.State = state;
            solvedTask.SolvedDate = DateTime.Now;

            await _taskRepository.Save();

            await _worflowHelper.ShareActivity(solvedTask.BlockModel.PoolId, solvedTask.WorkflowId, solvedTask.BlockModel.Pool.ModelId);
        }

        public async Task<ServiceTaskDetailPartialDTO> SaveServiceTask(IFormCollection data, IFormFileCollection files)
        {
            await SaveDataInternal(data, files);
            await _taskDataRepository.Save();

            return await ServiceTaskDetail(Guid.Parse(data["TaskId"]));
        }

        public async Task<ServiceTaskDetailPartialDTO> CallService(IFormCollection data, IFormFileCollection files)
        {
            Guid id = Guid.Parse(data["TaskId"]);
            await SaveDataInternal(data, files);
            _worflowHelper = new WorkflowHelper(_context);
            IServiceTaskWorkflowEntity serviceTask = await _blockWorkflowRepository.Bare(id) as IServiceTaskWorkflowEntity;
            serviceTask.FailedResponse = await _worflowHelper.CallService(serviceTask);
            await _taskDataRepository.Save();

            return await ServiceTaskDetail(id);
        }

        public async Task<TaskDataDTO> AddToArray(Guid taskDataId, DataTypeEnum type)
        {
            TaskDataEntity array = await _taskDataRepository.BareSchema(taskDataId);
            TaskDataEntity entity;
            switch (type)
            {
                case DataTypeEnum.String:
                    entity = new StringDataEntity();
                    break;
                
                case DataTypeEnum.Bool:
                    entity = new BoolDataEntity();
                    break;
                
                case DataTypeEnum.Number:
                    entity = new NumberDataEntity();
                    break;
                                
                default:
                    entity = new ArrayDataEntity();
                    break;
            }
            entity.SchemaId = array.SchemaId;
            entity.OutputTaskId = array.OutputTaskId;

            await _taskDataRepository.Create(entity);
            await _taskDataRepository.Save();

            return _mapper.Map(entity, entity.GetType(), typeof(TaskDataDTO)) as TaskDataDTO ?? new TaskDataDTO();
        }

        private async Task SaveDataInternal(IFormCollection data, IFormFileCollection files)
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
                    
                    case ISelectDataEntity selectData:
                        selectData.Value = valuePair.Value;
                        break;
                    
                    case IDateDataEntity dateData:
                        if (String.IsNullOrEmpty(valuePair.Value))
                        {
                            dateData.Value = null;
                            break;    
                        }
                        dateData.Value = DateTime.Parse(valuePair.Value);
                        break;
                    
                    case IFileDataEntity fileData:
                        File.Delete(StaticData.FileStore + fileData.Id);
                        fileData.FileName = null;
                        fileData.MIMEType = null;
                        break;
                }
            }

            foreach (IFormFile file in files)
            {
                 TaskDataEntity task = await _taskDataRepository.Detail(Guid.Parse(file.Name));
                 if (task is IFileDataEntity)
                 {
                     IFileDataEntity fileData = task as IFileDataEntity;
                     fileData.MIMEType = file.ContentType;
                     fileData.FileName = file.FileName;

                     using (FileStream fileStream = new FileStream(StaticData.FileStore + fileData.Id, FileMode.Create))
                     {
                         await file.CopyToAsync(fileStream);
                     }
                 }
            }
        }

        public async Task<TaskOverviewDTO> Overview(Guid userId)
        {
            return new TaskOverviewDTO()
            {
                Tasks = await _taskRepository.All()
            };
        }

        public async Task<List<TaskAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, UserId);
            _taskRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _taskRepository.All();
        }

        public async Task<UserTaskDetailDTO> UserTaskDetail(Guid id)
        {
            UserTaskDetailDTO detail = await UserTaskDetailPartial(id);
            detail.OtherTasks = await _taskRepository.All(id);
            detail.SelectedTask = await _taskRepository.Selected(id);

            return detail;
        }

        public async Task<UserTaskDetailDTO> UserTaskDetailPartial(Guid id)
        {
            UserTaskDetailDTO detail = await _taskRepository.UserDetail(id);
            
            List<TaskDataDTO> inputData = new List<TaskDataDTO>();
            foreach (TaskDataEntity data in await _taskDataMapRepository.MappedUserTaskData(id))
            {
                inputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
            }
            detail.InputData = inputData.GroupBy(x => x.BlockName);

            List<TaskDataDTO> outputData = new List<TaskDataDTO>();
            foreach (TaskDataEntity data in await _taskDataRepository.OutputUserTasks(id))
            {
                outputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
            }
            detail.OutputData = outputData.GroupBy(x => x.BlockName);

            List<TaskDataDTO> inputServiceData = new List<TaskDataDTO>();
            List<TaskDataDTO> outputServiceData = new List<TaskDataDTO>();
            foreach (TaskDataEntity data in await _taskDataRepository.MappedServiceTasks(id))
            {
                TaskDataDTO mapped = _mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO;
                if (data is IArrayDataEntity)
                {
                    int i = 0;
                    foreach (TaskDataEntity arrayData in await _taskDataRepository.OfArray(data.Id, data.SchemaId))
                    {
                        TaskDataDTO mappedArray = _mapper.Map(arrayData, arrayData.GetType(), typeof(TaskDataDTO)) as TaskDataDTO;
                        mappedArray.Name += $" - {++i}. hodnota";
                        (mapped as ITaskArray).Values.Add(mappedArray);
                    }
                }
                
                if (data.Schema.Direction == DirectionEnum.Input)
                {
                    inputServiceData.Add(mapped);
                }
                else
                {
                    outputServiceData.Add(mapped);
                }
            }
            detail.InputServiceData = inputServiceData.GroupBy(x => x.BlockName);
            detail.OutputServiceData = outputServiceData.GroupBy(x => x.BlockName);

            return detail;
        }

        public async Task<ServiceTaskDetailDTO> ServiceTaskDetailPartial(Guid id)
        {
            ServiceTaskDetailDTO detail = await _taskRepository.ServiceDetail(id);
            
            foreach (TaskDataEntity data in await _taskDataRepository.ServiceTaskData(id))
            {
                TaskDataDTO mapped = _mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO;
                if (data.Schema.Direction == DirectionEnum.Input)
                {
                    detail.InputData.Add(mapped);
                }
                else
                {
                    detail.OutputData.Add(mapped);
                }
            }

            return detail;
        }

        public async Task<ServiceTaskDetailDTO> ServiceTaskDetail(Guid id)
        {
            ServiceTaskDetailDTO detail = await ServiceTaskDetailPartial(id);
            detail.OtherTasks = await _taskRepository.All(id);
            detail.SelectedTask = await _taskRepository.Selected(id);

            return detail;
        }

        public async Task<FileDownloadDTO> DownloadFile(Guid id)
        {
            FileDownloadDTO file = await _taskDataRepository.FileForDownload(id);
            file.Data = await File.ReadAllBytesAsync(StaticData.FileStore + id);
            return file;
        }

        public void SetFilters(bool[] filters)
        {
            _taskRepository.Filters = filters;
            _taskRepository.UserId = UserId;
        }

        public struct TaskArrayCount
        {
            public TaskArrayDTO task;
            public int count;
        }
    }
}
