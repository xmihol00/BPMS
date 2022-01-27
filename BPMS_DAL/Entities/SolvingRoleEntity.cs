using BPMS_DAL.Entities.ModelBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class SolvingRoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<AgendaRoleUserEntity> UserRoles { get; set; } = new List<AgendaRoleUserEntity>();
        public List<UserTaskModelEntity> UserTasks { get; set; } = new List<UserTaskModelEntity>();
    }
}
