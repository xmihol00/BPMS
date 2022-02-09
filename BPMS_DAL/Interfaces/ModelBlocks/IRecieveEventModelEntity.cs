using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.ModelBlocks;

namespace BPMS_DAL.Interfaces.ModelBlocks
{
    public interface IRecieveEventModelEntity : IBlockModelEntity
    {
        public Guid? SenderId { get; set; }
        public SendEventModelEntity? Sender { get; set; }
        public bool Editable { get; set; }
    }
}
