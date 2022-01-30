using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.ModelBlocks;

namespace BPMS_DAL.Interfaces.ModelBlocks
{
    public interface ISendEventModelEntity : IBlockModelEntity
    {
        public List<RecieveEventModelEntity> Recievers { get; set; }
    }
}
