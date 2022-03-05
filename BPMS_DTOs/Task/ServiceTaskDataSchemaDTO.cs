using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task
{
    public class ServiceTaskDataSchemaDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<DataSchemaAttributeDTO> Attributes { get; set; } = new List<DataSchemaAttributeDTO>();
    }
}
