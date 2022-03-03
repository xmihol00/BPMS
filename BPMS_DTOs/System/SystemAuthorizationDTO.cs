using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemAuthorizationDTO
    {
        public Guid Id { get; set; }
        public string URL { get; set; } = string.Empty;
        public byte[]? Key { get; set; }
    }
}
