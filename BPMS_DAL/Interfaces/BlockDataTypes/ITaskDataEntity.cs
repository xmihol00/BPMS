using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.BlockDataTypes
{
    public interface ITaskDataEntity
    {
        public Guid Id { get; set; }
        public Guid OutputTaskId { get; set; }
        public BlockWorkflowEntity? OutputTask { get; set; }
        public List<TaskDataMapEntity> InputData { get; set; }
        public Guid? AttributeId { get; set; }
        public BlockAttributeEntity? Attribute { get; set; }
        public Guid? SchemaId { get; set; }
        public ServiceDataSchemaEntity? Schema { get; set; }
    }
}
