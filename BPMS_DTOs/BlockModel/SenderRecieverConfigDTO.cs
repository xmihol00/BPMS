using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel
{
    public class SenderRecieverConfigDTO
    {
        public string BlockName { get; set;} = string.Empty;
        public string PoolName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
    }
}
