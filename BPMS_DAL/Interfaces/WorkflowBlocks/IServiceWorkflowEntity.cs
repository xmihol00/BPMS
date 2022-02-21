using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.WorkflowBlocks
{
    public interface IServiceWorkflowEntity : IBlockWorkflowEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public List<TaskDataEntity> Data { get; set; }
    }
}