using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowAllAgendaDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkflowStateEnum State { get; set; }
        public string SVG { get; set; } = string.Empty;
        public string AdministratorName { get; set; } = string.Empty;
        public string AdministratorEmail { get; set; } = string.Empty;
        public Guid ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
    }
}
