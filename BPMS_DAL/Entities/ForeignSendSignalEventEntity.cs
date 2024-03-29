using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ForeignSendSignalEventEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SystemId { get; set; }
        public SystemEntity? System { get; set; }
        public Guid ForeignBlockId { get; set; }
        public string ForeignBlockName { get; set; } = string.Empty;
        public RecieveSignalEventModelEntity? Reciever { get; set; }
        public List<ForeignAttributeMapEntity> MappedAttributes { get; set; } = new List<ForeignAttributeMapEntity>();
    }
}
