using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class ServiceTaskModelEntity : BlockModelEntity, IServiceTaskModelEntity
    {
        public Guid ServiceId { get; set; }
        public ServiceEntity Service { get; set; } = new ServiceEntity();
    }
}
