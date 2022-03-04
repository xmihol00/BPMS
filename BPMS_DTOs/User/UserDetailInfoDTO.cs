using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserDetailInfoDTO
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<SystemRoleEnum> Roles { get; set; } = new List<SystemRoleEnum>();
    }
}
