using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Role
{
    public class RoleAllDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
