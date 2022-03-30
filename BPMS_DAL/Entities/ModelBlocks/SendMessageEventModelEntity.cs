using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class SendMessageEventModelEntity : BlockModelEntity, ISendMessageEventModelEntity
    {
        public SendMessageEventModelEntity() : base() {}
        public SendMessageEventModelEntity(PoolEntity pool) : base(pool) { }

        public Guid RecieverId { get; set; }

        [JsonIgnore]
        public RecieveMessageEventModelEntity? Reciever { get; set; }
    }
}
