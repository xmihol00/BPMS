using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class AttributeEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Specification { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public AttributeTypeEnum Type { get; set; }
        public Guid BlockId { get; set; }

        [JsonIgnore]
        public BlockModelEntity? Block { get; set; }

        [JsonIgnore]
        public Guid? ConditionId { get; set; }

        [JsonIgnore]
        public ConditionDataEntity? Condition { get; set; }

        [JsonIgnore]
        public List<TaskDataEntity> Data { get; set; } = new List<TaskDataEntity>();

        [JsonIgnore]
        public List<AttributeMapEntity> MappedBlocks { get; set; } = new List<AttributeMapEntity>();

        [JsonIgnore]
        public ForeignAttributeMapEntity? MappedForeignBlock { get; set; }
    }
}
