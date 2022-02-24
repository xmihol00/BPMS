using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Header;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceEditPagePartialDTO : ServiceEditPageHeaderDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ServiceTypeEnum Type { get; set; }
        public SerializationEnum Serialization { get; set; }
        public HttpMethodEnum HttpMethod { get; set; }
        public string URL { get; set; } = string.Empty;
        public IEnumerable<DataSchemaNodeDTO>? InputAttributes { get; set; }
        public IEnumerable<DataSchemaNodeDTO>? OutputAttributes { get; set; }
        public List<HeaderAllDTO> Headers { get; set; } = new List<HeaderAllDTO>();
    }
}
