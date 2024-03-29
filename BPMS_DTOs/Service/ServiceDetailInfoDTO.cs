using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Header;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceDetailInfoDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ServiceTypeEnum Type { get; set; }
        public SerializationEnum Serialization { get; set; }
        public HttpMethodEnum HttpMethod { get; set; }
        public ServiceAuthEnum AuthType { get; set; }
        public string URL { get; set; } = string.Empty;
        public Guid Id { get; set; }
    }
}
