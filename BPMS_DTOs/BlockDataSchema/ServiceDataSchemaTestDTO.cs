using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.ServiceDataSchema
{
    public class ServiceDataSchemaTestDTO
    {
        public Guid ServiceId { get; set; }
        public List<ServiceDataSchemaAllDTO> Schemas { get; set; } = new List<ServiceDataSchemaAllDTO>();
    }
}
