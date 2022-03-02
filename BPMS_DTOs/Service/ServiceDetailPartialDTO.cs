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
    public class ServiceDetailPartialDTO : ServiceDetailHeaderDTO
    {
        public IEnumerable<DataSchemaNodeDTO>? InputAttributes { get; set; }
        public IEnumerable<DataSchemaNodeDTO>? OutputAttributes { get; set; }
        public List<HeaderAllDTO> Headers { get; set; } = new List<HeaderAllDTO>();
    }
}
