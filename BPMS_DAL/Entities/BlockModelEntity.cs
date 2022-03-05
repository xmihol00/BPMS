using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class BlockModelEntity : IBlockModelEntity
    {
        public BlockModelEntity() {}
        public BlockModelEntity(PoolEntity pool)
        {
            PoolId = pool.Id;
            Pool = pool;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public uint Order { get; set; }
        public Guid PoolId { get; set; }

        [JsonIgnore]
        public PoolEntity? Pool { get; set; }

        [JsonIgnore]
        public List<FlowEntity> InFlows { get; set; } = new List<FlowEntity>();
        
        [JsonIgnore]
        public List<FlowEntity> OutFlows { get; set; } = new List<FlowEntity>();

        [JsonIgnore]
        public List<BlockWorkflowEntity> BlockWorkflows { get; set; } = new List<BlockWorkflowEntity>();

        [JsonIgnore]
        public List<BlockAttributeEntity> Attributes { get; set; } = new List<BlockAttributeEntity>();

        [JsonIgnore]
        public List<BlockAttributeMapEntity> MappedAttributes { get; set; } = new List<BlockAttributeMapEntity>();

        [JsonIgnore]
        public List<BlockModelDataSchemaEntity> DataSchemas { get; set; } = new List<BlockModelDataSchemaEntity>();
    }
}
