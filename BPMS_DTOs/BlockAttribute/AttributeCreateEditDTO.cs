using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockAttribute
{
    public class AttributeCreateEditDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Specification { get; set; } = new List<string>();
        public string? Compulsory { get; set; }
        public AttributeTypeEnum Type { get; set; }
        public Guid TaskId { get; set; }
    }
}