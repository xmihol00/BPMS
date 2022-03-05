using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task
{
    public class UserTaskDetailPartialDTO : UserTaskDetailHeaderDTO
    {
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
        public Guid AgendaId { get; set; }
        public string AgendaName { get; set; } = string.Empty;
        public TaskPriorityEnum Priority { get; set; }
        public DateTime SolveDate { get; set; }
        public IEnumerable<IGrouping<string, TaskDataDTO>> InputData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
        public IEnumerable<IGrouping<string, TaskDataDTO>> OutputData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
        public IEnumerable<IGrouping<string, TaskDataDTO>> InputServiceData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
        public IEnumerable<IGrouping<string, TaskDataDTO>> OutputServiceData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
    }
}
