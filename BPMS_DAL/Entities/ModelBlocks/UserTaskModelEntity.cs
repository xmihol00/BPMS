using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class UserTaskModelEntity : BlockModelEntity, IUserTaskModelEntity
    {
        public Guid RoleId { get; set; }
        public SolvingRoleEntity Role { get; set; } = new SolvingRoleEntity();
        public TimeSpan Span { get; set; }
    }
}
