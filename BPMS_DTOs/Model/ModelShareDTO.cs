using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.Flow;
using BPMS_DTOs.Pool;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelShareDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SVG { get; set; } = string.Empty;
        public ModelStateEnum State { get; set; }
        public List<PoolShareDTO> Pools { get; set; } = new List<PoolShareDTO>();
        public List<FlowShareDTO> Flows  { get; set; } = new List<FlowShareDTO>();
    }
}
