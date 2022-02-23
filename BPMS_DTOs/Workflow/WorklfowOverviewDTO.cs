using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowOverviewDTO
    {
        public List<WorkflowAllDTO> Workflows { get; set; } = new List<WorkflowAllDTO>();
    }
}
