using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;

namespace BPMS_DAL.Entities
{
    public class BlockModelDataSchemaEntity
    {
        public Guid BlockId { get; set; }
        public BlockModelEntity? Block { get; set; }
        public Guid DataSchemaId { get; set; }
        public ServiceDataSchemaEntity? DataSchema { get; set; }
        public Guid ServiceTaskId { get; set; }
        public ServiceTaskModelEntity? ServiceTask { get; set; }
    }
}
