using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class BlockDataSchemaEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public BlockTypeEnum BlockType { get; set; }
        public Guid? ParentId { get; set; }
        public BlockDataSchemaEntity Parent { get; set; } = new BlockDataSchemaEntity();
        public List<BlockDataSchemaEntity> Children { get; set; } = new List<BlockDataSchemaEntity>();
        public List<ConditionDataEntity> Conditions { get; set; } = new List<ConditionDataEntity>();
    }
}
