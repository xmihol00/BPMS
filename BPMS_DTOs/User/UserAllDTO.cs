using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserAllDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public List<SystemRoleEnum> Roles { get; set; } = new List<SystemRoleEnum>();
    }
}
