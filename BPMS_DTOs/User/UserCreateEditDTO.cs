using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserCreateEditDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<SystemRoleEnum> RemovedRoles { get; set; } = new List<SystemRoleEnum>();
        public List<SystemRoleEnum> AddedRoles { get; set; } = new List<SystemRoleEnum>();
    }
}
