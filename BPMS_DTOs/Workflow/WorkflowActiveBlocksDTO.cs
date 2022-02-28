using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowActiveBlocksDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        
        [JsonProperty("blockIds")]
        public List<Guid> BlockIds { get; set; } = new List<Guid>();
    }
}
