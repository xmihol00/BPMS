using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow.ConfigTypes
{
    public class UserTaskWorkflowConfigDTO : TaskWorkflowConfigDTO, IUserTaskWorkflowConfigDTO
    {
        public TaskPriorityEnum Priority { get; set; }
        public DateTime SolveDate { get; set; }
    }
}
