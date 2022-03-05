using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.BlockDataTypes;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class TaskDataEntity : ITaskDataEntity
    {
        public Guid Id { get; set; }
        public Guid OutputTaskId { get; set; }
        
        [JsonIgnore]
        public BlockWorkflowEntity? OutputTask { get; set; }
        
        [JsonIgnore]
        public List<TaskDataMapEntity> InputData { get; set; } = new List<TaskDataMapEntity>();
        public Guid? AttributeId { get; set; }

        [JsonIgnore]
        public AttributeEntity? Attribute { get; set; }
        public Guid? SchemaId { get; set; }

        [JsonIgnore]
        public DataSchemaEntity? Schema { get; set; }
    }
}
