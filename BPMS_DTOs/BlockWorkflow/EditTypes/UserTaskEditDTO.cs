using BPMS_Common.Enums;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow.EditTypes
{
    public class UserTaskEditDTO : BlockWorkflowEditDTO
    {
        public DateTime SolveDate { get; set; }
        public TaskPriorityEnum Priority { get; set; }
    }
}
