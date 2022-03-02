using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class TaskModelEntity : BlockModelEntity
    {
        public TaskModelEntity() : base() {}
        public TaskModelEntity(PoolEntity pool) : base(pool) { }

        public Guid? RoleId { get; set; }
        public SolvingRoleEntity? Role { get; set; }
    }
}
