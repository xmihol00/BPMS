using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceCallResultDTO
    {
        public Guid ServiceId { get; set; }
        public string RecievedData { get; set; } = string.Empty;
        public SerializationEnum? Serialization { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
