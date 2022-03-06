using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemEditDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}