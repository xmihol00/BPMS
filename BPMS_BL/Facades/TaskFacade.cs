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

        public TaskFacade(BlockWorkflowRepository taskRepository, TaskDataRepository taskDataRepository)
        {
            _taskRepository = taskRepository;
            _taskDataRepository = taskDataRepository;
        }

        public async Task<TaskOverviewDTO> Overview(Guid userId)
        {
            return new TaskOverviewDTO()
            {
                Tasks = await _taskRepository.Overview(userId)
            };
        }

        public async Task<TaskDetailDTO> Detail(Guid id, TaskTypeEnum type, Guid userId)
        {
            TaskDetailDTO detail = await _taskRepository.Detail(id, userId);

            if (type == TaskTypeEnum.UserTask)
            {
                detail.Data = await CreateData(await _taskRepository.DataUser(id));
            }
            else if (type == TaskTypeEnum.ServiceTask)
            {
                detail.Data = await CreateData(await _taskRepository.DataService(id));
            }

            return detail;
        }

        private Task<List<TaskDataDTO>> CreateData(BlockWorkflowEntity data)
        {
            throw new NotImplementedException();
        }
    }
}
