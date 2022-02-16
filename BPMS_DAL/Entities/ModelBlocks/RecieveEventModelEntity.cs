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
    public class RecieveEventModelEntity : BlockModelEntity, IRecieveEventModelEntity
    {
        public RecieveEventModelEntity() : base() {}
        public RecieveEventModelEntity(PoolEntity pool) : base(pool) { }

        public Guid? SenderId { get; set; }

        [JsonIgnore]
        public SendEventModelEntity? Sender { get; set; }
        public bool Editable { get; set; } = false;
    }
}
