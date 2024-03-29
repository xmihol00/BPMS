using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Pool
{
    public class PoolEditDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? SystemId { get; set; }
        public Guid? LaneId { get; set; }
        public Guid? RoleId { get; set; }
    }
}
