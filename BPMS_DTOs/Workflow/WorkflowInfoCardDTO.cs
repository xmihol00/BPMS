using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowInfoCardDTO : WorkflowDetailInfoDTO 
    {
        public WorkflowAllDTO SelectedWorkflow { get; set; } = new WorkflowAllDTO();
    }
}
