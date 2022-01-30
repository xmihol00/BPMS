using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class RecieveEventModelEntity : BlockModelEntity, IRecieveEventModelEntity
    {
        public Guid SenderId { get; set; }
        public SendEventModelEntity Sender { get; set; } = new SendEventModelEntity();
    }
}
