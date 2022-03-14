using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        public string? Description { get; set; }
        public Guid ModelId { get; set; }
        public Guid? StartedId { get; set; }

        [JsonIgnore]
        public ModelEntity? Model { get; set; }

        [JsonIgnore]
        public Guid? SystemId { get; set; }

        [JsonIgnore]
        public SystemEntity? System { get; set; }
        public List<BlockModelEntity> Blocks { get; set; } = new List<BlockModelEntity>();
    }
}
