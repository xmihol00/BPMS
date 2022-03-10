using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.DataSchema
{
    public class DataSchemaMappedDTO
    {
        public Guid ServiceTaskId { get; set; }
        public DataSchemaMapDTO Source { get; set; } = new DataSchemaMapDTO();
        public DataSchemaMapDTO Target { get; set; } = new DataSchemaMapDTO();
    }
}
