using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class SystemRoleEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public SystemRoleEnum Role { get; set; }
    }
}
