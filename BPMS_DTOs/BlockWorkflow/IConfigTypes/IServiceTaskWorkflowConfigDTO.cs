using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow.IConfigTypes
{
    public interface IServiceTaskWorkflowConfigDTO : ITaskWorkflowConfigDTO
    {
        public string ServiceName { get; set; }
    }
}
