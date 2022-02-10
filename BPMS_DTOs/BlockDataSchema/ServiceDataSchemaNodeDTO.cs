using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.ServiceDataSchema
{
    public class ServiceDataSchemaNodeDTO
    {
        public IEnumerable<ServiceDataSchemaNodeDTO>? Children { get; set; }
        public Guid? ParentId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string? StaticData { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public DataTypeEnum Type { get; set; }
    }
}
