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
    public class ForeignAttributeMapEntity
    {
        public Guid ForeignSendEventId { get; set; }
        public ForeignSendSignalEventEntity? ForeignSendEvent { get; set; }
        public Guid AttributeId { get; set; }
        public AttributeEntity? Attribute { get; set; }
        public Guid ForeignAttributeId { get; set; }
    }
}
