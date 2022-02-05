using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockDataSchema
{
    public class BlockDataSchemaNodeDTO
    {
        public BlockDataSchemaNodeDTO? Parent { get; set; }
        public IEnumerable<BlockDataSchemaNodeDTO>? Children { get; set; }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public DataTypeEnum DataType { get; set; }
    }
}