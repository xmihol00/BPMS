using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockWorkflow
{
    public class BlockWorkflowEditDTO
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set;}
    }
}
