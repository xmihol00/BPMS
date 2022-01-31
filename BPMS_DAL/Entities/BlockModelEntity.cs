using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;

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
        public string Description { get; set; } = string.Empty;
        public Guid PoolId { get; set; }
        public PoolEntity? Pool { get; set; }
        public List<FlowEntity> InFlows { get; set; } = new List<FlowEntity>();
        public List<FlowEntity> OutFlows { get; set; } = new List<FlowEntity>();
        public List<BlockWorkflowEntity> BlockWorkflows { get; set; } = new List<BlockWorkflowEntity>();
        public List<BlockDataSchemaEntity> DataSchemas { get; set; } = new List<BlockDataSchemaEntity>();
    }
}
