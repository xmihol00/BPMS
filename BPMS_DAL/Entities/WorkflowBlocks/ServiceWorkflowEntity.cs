using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.WorkflowBlocks;

namespace BPMS_DAL.Entities.WorkflowBlocks
{
    public class ServiceWorkflowEntity : BlockWorkflowEntity, IServiceWorkflowEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public List<TaskDataEntity> Data { get; set; } = new List<TaskDataEntity>();
    }
}
