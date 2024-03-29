using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceCreateEditDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public ServiceTypeEnum Type { get; set; }
        public SerializationEnum Serialization { get; set; }
        public HttpMethodEnum HttpMethod { get; set; }
        public Uri? URL { get; set; }
        public ServiceAuthEnum AuthType { get; set; }
        public string? AppId { get; set; }
        public string? AppSecret { get; set; }
    }
}
