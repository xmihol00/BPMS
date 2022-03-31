using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Role;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Lane
{
    public class LaneEditDTO
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }
}
