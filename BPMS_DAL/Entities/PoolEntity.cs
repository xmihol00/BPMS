using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class PoolEntity
    {
        public PoolEntity() {}
        public PoolEntity(ModelEntity model) 
        {
            ModelId = model.Id;
            Model = model;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ModelId { get; set; }
        public ModelEntity? Model { get; set; }
        public List<SystemPoolEntity> Systems { get; set; } = new List<SystemPoolEntity>();
        public List<BlockModelEntity> Blocks { get; set; } = new List<BlockModelEntity>();
    }
}
