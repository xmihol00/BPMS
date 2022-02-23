using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class BlockAttributeEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Specification { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public AttributeTypeEnum Type { get; set; }
        public Guid BlockId { get; set; }
        public BlockModelEntity? Block { get; set; }
        public Guid? ConditionId { get; set; }
        public ConditionDataEntity? Condition { get; set; }
        public List<TaskDataEntity> Data { get; set; } = new List<TaskDataEntity>();
        public List<BlockAttributeMapEntity> MappedAttributes { get; set; } = new List<BlockAttributeMapEntity>();
    }
}
