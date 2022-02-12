using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.ServiceDataSchema
{
    public class DataSchemaCreateEditDTO
    {
        public Guid Id { get; set; }
        public Guid BlockId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public uint Order { get; set; }
        public string? StaticData { get; set; }
        public string? Compulsory { get; set; }
        public string? DataToggle { get; set; }
        public DataTypeEnum Type { get; set; }
        public Guid ServiceId { get; set; }
        public DirectionEnum Direction { get; set; }
    }
}
