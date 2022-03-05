using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class AttributeMapEntity
    {
        public Guid BlockId { get; set; }

        [JsonIgnore]
        public BlockModelEntity? Block { get; set; }
        public Guid AttributeId { get; set; }

        [JsonIgnore]
        public AttributeEntity? Attribute { get; set; }
    }
}
