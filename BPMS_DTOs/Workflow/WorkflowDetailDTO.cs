using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowDetailDTO : WorkflowDetailPartialDTO
    {
        public List<WorkflowAllDTO> OtherWorkflows { get; set; } = new List<WorkflowAllDTO>();
        public WorkflowAllDTO SelectedWorkflow { get; set; } = new WorkflowAllDTO();
    }
}
