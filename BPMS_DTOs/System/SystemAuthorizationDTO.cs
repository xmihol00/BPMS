using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemAuthorizationDTO
    {
        public Guid MessageId { get; set; }
        public string URL { get; set; } = string.Empty;
        public byte[]? PayloadHash { get; set; }
        public byte[]? PayloadKey { get; set; }
        public byte[]? PayloadIV { get; set; }
    }
}
