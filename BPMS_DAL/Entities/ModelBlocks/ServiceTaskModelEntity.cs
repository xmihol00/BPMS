using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class ServiceTaskModelEntity : BlockModelEntity
    {
        public Guid ServiceId { get; set; }
        public ServiceEntity Service { get; set; } = new ServiceEntity();
    }
}
