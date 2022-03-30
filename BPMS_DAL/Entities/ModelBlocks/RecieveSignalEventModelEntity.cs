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
    public class RecieveSignalEventModelEntity : BlockModelEntity, IRecieveSignalEventModelEntity
    {
        public RecieveSignalEventModelEntity() : base() {}
        public RecieveSignalEventModelEntity(PoolEntity pool) : base(pool) { }
        
        public Guid? ForeignSenderId { get; set; }
        public ForeignSendSignalEventEntity? ForeignSender { get; set; }
    }
}
