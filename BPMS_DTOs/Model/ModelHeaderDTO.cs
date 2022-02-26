using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelHeaderDTO : ModelInfoDTO
    {
        public Guid Id { get; set; }
        public ModelStateEnum State { get; set; }
        public WorkflowRunDTO? Workflow { get; set; }
    }
}
