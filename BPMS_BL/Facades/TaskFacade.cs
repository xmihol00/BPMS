using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.Pool;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
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
        private readonly PoolRepository _poolRepository;
        private readonly BpmsDbContext _context;
        private WorkflowHelper _worflowHelper { get; set; }
        private readonly IMapper _mapper;

        #pragma warning disable CS8618
        public TaskFacade(BlockWorkflowRepository taskRepository, TaskDataRepository taskDataRepository, 
                          BlockModelRepository blockModelRepository, AgendaRoleRepository agendaRoleRepository, 
                          ServiceDataSchemaRepository serviceDataSchemaRepository, ServiceRepository serviceRepository, 
                          WorkflowRepository workflowRepository, PoolRepository poolRepository, BpmsDbContext context,
                          IMapper mapper)
        {
            _taskRepository = taskRepository;
            _taskDataRepository = taskDataRepository;
            _blockModelRepository = blockModelRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
            _serviceRepository = serviceRepository;
            _workflowRepository = workflowRepository;
            _poolRepository = poolRepository;
            _context = context;
            _mapper = mapper;
        }
#pragma warning restore CS8618

        public async Task<UserTaskDetailPartialDTO> SaveData(IFormCollection data, IFormFileCollection files, Guid userId)
        {
            await SaveDataInternal(data, files);
            await _taskDataRepository.Save();

            return await UserTaskDetail(Guid.Parse(data["TaskId"]), userId);
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
                        fileData.FileName = "";
                        fileData.MIMEType = "";
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

        public async Task SolveUserTask(IFormCollection data, IFormFileCollection files)
        {
            await SaveDataInternal(data, files);
            _worflowHelper = new WorkflowHelper(_context);

            BlockWorkflowEntity solvedTask = await _taskRepository.TaskForSolving(Guid.Parse(data["TaskId"]));
            await _worflowHelper.StartNextTask(solvedTask);

            solvedTask.Active = false;
            solvedTask.SolvedDate = DateTime.Now;

            await _taskRepository.Save();

            await _worflowHelper.ShareActivity(solvedTask.BlockModel.PoolId, solvedTask.WorkflowId, solvedTask.BlockModel.Pool.ModelId);
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
            
            List<TaskDataDTO> inputData = new List<TaskDataDTO>();
            foreach (TaskDataEntity data in await _taskDataRepository.MappedUserTaskData(id))
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
                if (data.Schema.Direction == DirectionEnum.Input)
                {
                    inputServiceData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
                else
                {
                    outputServiceData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
            }
            detail.InputServiceData = inputServiceData.GroupBy(x => x.BlockName);
            detail.OutputServiceData = outputServiceData.GroupBy(x => x.BlockName);

            return detail;
        }

        public async Task<FileDownloadDTO> DownloadFile(Guid id)
        {
            FileDownloadDTO file = await _taskDataRepository.FileForDownload(id);
            file.Data = await File.ReadAllBytesAsync(StaticData.FileStore + id);
            return file;
        }
    }
}
