using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Sharing
{
    public class WorkflowShare
    {
        public Guid ModelId { get; set; }
        public WorkflowEntity Workflow { get; set; } = new WorkflowEntity();
    }
}
