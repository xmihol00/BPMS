using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Text { get; set; } = string.Empty;
        public EncryptionLevelEnum Encryption { get; set; }
    }
}
