using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BPMS_BL.Facades
{
    public class TaskFacade
    {
        private readonly BlockWorkflowRepository _taskRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly IMapper _mapper;

        public TaskFacade(BlockWorkflowRepository taskRepository, TaskDataRepository taskDataRepository, 
                          BlockModelRepository blockModelRepository, AgendaRoleRepository agendaRoleRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _taskDataRepository = taskDataRepository;
            _blockModelRepository = blockModelRepository;
            _agendaRoleRepository = agendaRoleRepository;
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
                        boolData.Value = Boolean.Parse(valuePair.Value);
                        break;
                    
                    case IArrayDataEntity arrayData:
                        break;
                    
                    case INumberDataEntity numberData:
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
            BlockWorkflowEntity solvedTask = await _taskRepository.Bare(Guid.Parse(data["TaskId"]));

            foreach (BlockWorkflowEntity task in await _taskRepository.NextBlocks(solvedTask.Id, solvedTask.WorkflowId))
            {
                switch (task)
                {
                    case IUserTaskWorkflowEntity userTask:
                        userTask.Active = true;
                        UserTaskModelEntity userTaskModel = await _blockModelRepository.UserTaskForSolve(userTask.BlockModelId);
                        userTask.SolveDate = DateTime.Now.AddDays(userTaskModel.Difficulty.TotalDays);
                        userTask.UserId = await _agendaRoleRepository.LeastBussyUser(userTaskModel.RoleId ?? Guid.Empty) ?? 
                                          userTaskModel.Pool.Model.Agenda.AdministratorId;
                        break;
                    
                    default:
                        throw new NotImplementedException();
                }

                _taskRepository.Update(task);
            }
            
            solvedTask.Active = false;
            solvedTask.SolvedDate = DateTime.Now;

            await _taskRepository.Save();
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

            foreach (TaskDataEntity data in await _taskDataRepository.Output(id))
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
