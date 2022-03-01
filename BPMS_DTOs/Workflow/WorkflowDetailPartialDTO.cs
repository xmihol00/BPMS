using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Workflow
{
    public class WorkflowDetailPartialDTO : WorkflowDetailInfoDTO
    {
        public string SVG { get; set; } = string.Empty;
        public Guid AgendaId { get; set; }
        public string AgendaName { get; set; } = string.Empty;
        public Guid ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public WorkflowActiveBlocksDTO ActiveBlocks { get; set; } = new WorkflowActiveBlocksDTO();
        public IEnumerable<IGrouping<string, TaskDataDTO>> OutputData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
        public IEnumerable<IGrouping<string, TaskDataDTO>> InputServiceData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
        public IEnumerable<IGrouping<string, TaskDataDTO>> OutputServiceData { get; set; } = new List<IGrouping<string, TaskDataDTO>>();
    }
}
