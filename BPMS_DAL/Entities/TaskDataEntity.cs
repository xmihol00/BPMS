using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.WorkflowBlocks;

namespace BPMS_DAL.Entities
{
    public class TaskDataEntity
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public TaskWorkflowEntity? Task { get; set; }
    }
}
