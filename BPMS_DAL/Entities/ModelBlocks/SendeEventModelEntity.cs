using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class SendeEventModelEntity : BlockModelEntity
    {
        public List<RecieveEventModelEntity> Recievers { get; set; } = new List<RecieveEventModelEntity>();
    }
}
