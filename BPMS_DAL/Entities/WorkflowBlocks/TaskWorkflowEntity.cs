using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.WorkflowBlocks
{
    public class TaskWorkflowEntity : BlockWorkflowEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public DateTime SolveDate { get; set; }
        public List<TaskDataEntity> Data { get; set; } = new List<TaskDataEntity>();
    }
}
