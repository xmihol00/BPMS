using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.WorkflowBlocks;

namespace BPMS_DAL.Entities
{
    public class TaskDataMapEntity
    {
        public Guid TaskId { get; set; }
        public BlockWorkflowEntity? Task { get; set; }
        public Guid TaskDataId { get; set; }
        public TaskDataEntity? TaskData { get; set; }
    }
}
