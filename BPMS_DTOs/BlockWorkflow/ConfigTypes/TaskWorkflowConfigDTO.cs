using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.Task;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow.ConfigTypes
{
    public class TaskWorkflowConfigDTO : BlockWorkflowConfigDTO, ITaskWorkflowConfigDTO, IOutputData
    {
        public Guid? CurrentUserId { get; set; }
        public List<UserIdNameDTO> UserIdNames { get; set; } = new List<UserIdNameDTO>();
        public BlockWorkflowStateEnum State { get; set; }
        public List<TaskDataDTO> OutputData { get; set; } = new List<TaskDataDTO>();
    }
}
