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
    public class PoolShareDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ModelId { get; set; }
        public string SystemURL { get; set; } = string.Empty;
    }
}
