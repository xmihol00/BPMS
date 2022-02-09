using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceAllDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ServiceTypeEnum Type { get; set; }
        public SerializationEnum Serialization { get; set; }
        public HttpMethodEnum HttpMethod { get; set; }
        public string URL { get; set; } = string.Empty;
    }
}
