using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task
{
    public class TaskOverviewDTO
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
        public Guid AgendaId { get; set; }
        public string AgendaName { get; set; } = string.Empty;
    }
}
