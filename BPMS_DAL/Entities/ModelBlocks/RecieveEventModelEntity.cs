using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class RecieveEventModelEntity : BlockModelEntity
    {
        public Guid SenderId { get; set; }
        public SendeEventModelEntity Sender { get; set; } = new SendeEventModelEntity();
    }
}
