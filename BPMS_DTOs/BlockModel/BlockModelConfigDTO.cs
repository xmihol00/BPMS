using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.BlockDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel
{
    public class BlockModelConfigDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<BlockDataSchemaNodeDTO> RootNodes { get; set; } = new List<BlockDataSchemaNodeDTO>();
    }
}
