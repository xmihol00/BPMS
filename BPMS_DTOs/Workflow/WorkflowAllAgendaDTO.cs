using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowAllAgendaDTO : WorkflowAllModelDTO
    {
        public Guid ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string SVG { get; set; } = string.Empty;
    }
}
