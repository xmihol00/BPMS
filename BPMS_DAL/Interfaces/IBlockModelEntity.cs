using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces
{
    public interface IBlockModelEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid PoolId { get; set; }
        public PoolEntity? Pool { get; set; }
        public List<FlowEntity> InFlows { get; set; }
        public List<FlowEntity> OutFlows { get; set; }
        public List<BlockWorkflowEntity> BlockWorkflows { get; set; }
        public List<BlockAttributeEntity> Attributes { get; set; }
        public List<BlockAttributeMapEntity> MappedAttributes { get; set; }
    }
}
