using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockModel.ConfigTypes;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.System;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Authentication;

namespace BPMS_BL.Facades
{
    public class TaskFacade
    {
        private readonly BlockWorkflowRepository _taskRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly IMapper _mapper;

        public TaskFacade(BlockWorkflowRepository taskRepository, TaskDataRepository taskDataRepository, 
                          BlockModelRepository blockModelRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _taskDataRepository = taskDataRepository;
            _blockModelRepository = blockModelRepository;
            _mapper = mapper;
        }

        public Task<object?> ServiceDetail(Guid id, Guid userId)
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

        public async Task<UserTaskDetailDTO> UserDetail(Guid id, Guid userId)
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
