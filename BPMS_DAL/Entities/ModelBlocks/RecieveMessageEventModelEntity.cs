using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class RecieveMessageEventModelEntity : BlockModelEntity, IRecieveMessageEventModelEntity
    {
        public RecieveMessageEventModelEntity() : base() {}
        public RecieveMessageEventModelEntity(PoolEntity pool) : base(pool) { }

        [JsonIgnore]
        public SendMessageEventModelEntity? Sender { get; set; }
    }
}
