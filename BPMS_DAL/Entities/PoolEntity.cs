using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class PoolEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ModelId { get; set; }
        public ModelEntity Model { get; set; } = new ModelEntity();
        public List<SystemPoolEntity> Systems { get; set; } = new List<SystemPoolEntity>();
        public List<BlockModelEntity> Blocks { get; set; } = new List<BlockModelEntity>();
    }
}
