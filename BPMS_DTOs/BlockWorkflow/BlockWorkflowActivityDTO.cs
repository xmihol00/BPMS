using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow
{
    public class BlockWorkflowActivityDTO
    {
        public Guid WorkflowId { get; set; }
        public Guid BlockModelId { get; set; }
        public bool Active { get; set; }
    }
}
