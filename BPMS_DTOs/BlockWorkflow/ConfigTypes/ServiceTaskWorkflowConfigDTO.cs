using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow.ConfigTypes
{
    public class ServiceTaskWorkflowConfigDTO : TaskWorkflowConfigDTO, IServiceTaskWorkflowConfigDTO
    {
        public string ServiceName { get; set; } = string.Empty;
    }
}
