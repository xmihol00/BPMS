using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class SendSignalEventModelEntity : BlockModelEntity, ISendSignalEventModelEntity
    {
        public SendSignalEventModelEntity() : base() {}
        public SendSignalEventModelEntity(PoolEntity pool) : base(pool) { }
        
        [JsonIgnore]
        public List<ForeignSignalRecieveEventEntity> ForeignRecievers { get; set; } = new List<ForeignSignalRecieveEventEntity>();
    }
}
