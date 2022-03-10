using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.DataSchema
{
    public class DataSchemaSourceDTO
    {
        public string BlockName { get; set; } = string.Empty;
        public List<DataSchemaMapDTO> Sources { get; set; } = new List<DataSchemaMapDTO>();
    }
}
