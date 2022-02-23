using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.ServiceDataSchema
{
    public class DataSchemaAttributeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public DataTypeEnum Type { get; set; }
        public bool Mapped { get; set; }
    }
}
