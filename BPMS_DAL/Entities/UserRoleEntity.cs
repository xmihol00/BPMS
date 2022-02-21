using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class UserRoleEntity
    {
        public Guid AgendaRoleId { get; set; }
        public Guid UserId { get; set; }
        public AgendaRoleEntity? AgendaRole { get; set; }
        public UserEntity? User { get; set; }
    }
}
