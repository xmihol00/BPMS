using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class AgendaRoleEntity
    {
        public Guid Id { get; set; }
        public Guid AgendaId { get; set; }
        public Guid RoleId { get; set; }
        public AgendaEntity? Agenda { get; set; }
        public SolvingRoleEntity? Role { get; set; }
        public List<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
