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
    public class ForeignSignalRecieveEventEntity
    {
        public Guid SenderId { get; set; }
        public SendSignalEventModelEntity? Sender { get; set; }
        public Guid SystemId { get; set; }
        public SystemEntity? System { get; set; }
        public Guid ForeignBlockId { get; set; }
        public string ForeignBlockName { get; set; } = string.Empty;
    }
}
