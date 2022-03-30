using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.WorkflowBlocks;

namespace BPMS_DAL.Entities.WorkflowBlocks
{
    public class RecieveSignalEventWorkflowEntity : BlockWorkflowEntity, IRecieveSignalEventWorkflowEntity
    {
        public bool Delivered { get; set; }
    }
}
