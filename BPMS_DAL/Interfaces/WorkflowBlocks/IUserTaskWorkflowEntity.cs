using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.WorkflowBlocks
{
    public interface IUserTaskWorkflowEntity : ITaskWorkflowEntity
    {
        public DateTime SolveDate { get; set; }
        public TaskPriorityEnum Priority { get; set; }
    }
}
