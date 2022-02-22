using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.WorkflowBlocks;

namespace BPMS_DAL.Entities
{
    public class TaskDataEntity
    {
        public Guid Id { get; set; }
        public Guid OutputTaskId { get; set; }
        public BlockWorkflowEntity? OutputTask { get; set; }
        public List<TaskDataMapEntity> InputData { get; set; } = new List<TaskDataMapEntity>();
        public Guid? AttributeId { get; set; }
        public BlockAttributeEntity? Attribute { get; set; }
        public Guid? SchemaId { get; set; }
        public ServiceDataSchemaEntity? Schema { get; set; }
    }
}
