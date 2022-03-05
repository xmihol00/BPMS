using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.DataSchema
{
    public class DataSchemaTestDTO
    {
        public Guid ServiceId { get; set; }
        public List<DataSchemaAllDTO> Schemas { get; set; } = new List<DataSchemaAllDTO>();
    }
}
