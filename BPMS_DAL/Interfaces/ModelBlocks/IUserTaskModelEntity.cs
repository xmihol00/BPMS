using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.ModelBlocks
{
    public interface IUserTaskModelEntity : IBlockModelEntity
    {
        public Guid? RoleId { get; set; }
        public SolvingRoleEntity? Role { get; set; }
        public TimeSpan Difficulty { get; set; }
    }
}
