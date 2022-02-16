using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class SendEventModelEntity : BlockModelEntity, ISendEventModelEntity
    {
        public SendEventModelEntity() : base() {}
        public SendEventModelEntity(PoolEntity pool) : base(pool) { }

        [JsonIgnore]
        public List<RecieveEventModelEntity> Recievers { get; set; } = new List<RecieveEventModelEntity>();
    }
}
