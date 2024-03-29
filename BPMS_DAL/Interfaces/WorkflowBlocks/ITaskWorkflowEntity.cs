using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.WorkflowBlocks
{
    public interface ITaskWorkflowEntity : IBlockWorkflowEntity
    {
        public Guid? UserId { get; set; }
        public UserEntity? User { get; set; }
    }
}
