using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class AgendaRoleUserEntity
    {
        public Guid AgendaId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public AgendaEntity Agenda { get; set; } = new AgendaEntity();
        public UserEntity User { get; set; } = new UserEntity();
        public SolvingRoleEntity Role { get; set; } = new SolvingRoleEntity();
    }
}
