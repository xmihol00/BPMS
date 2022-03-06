using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow
{
    public class BlockWorkflowActivityDTO
    {
        public Guid WorkflowId { get; set; }
        public Guid BlockModelId { get; set; }
        public BlockWorkflowStateEnum State { get; set; }
    }
}
