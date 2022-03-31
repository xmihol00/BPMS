using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Role;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Lane
{
    public class LaneConfigDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<RoleAllDTO> Roles { get; set; } = new List<RoleAllDTO>();
        public Guid? CurrentRoleId { get; set; }
    }
}
