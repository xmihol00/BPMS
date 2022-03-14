using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelDetailPartialDTO : ModelDetailInfoDTO
    {
        public string SVG { get; set; } = string.Empty;
        public List<WorkflowAllModelDTO> Workflows { get; set; } = new List<WorkflowAllModelDTO>();
        public List<WorkflowActiveBlocksDTO> ActiveBlocks { get; set; } = new List<WorkflowActiveBlocksDTO>();
        public List<Guid> ActivePools { get; set; } = new List<Guid>();
    }
}
