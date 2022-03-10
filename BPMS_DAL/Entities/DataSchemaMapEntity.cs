using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class DataSchemaMapEntity
    {
        public Guid ServiceTaskId { get; set; }
        public ServiceTaskModelEntity? ServiceTask { get; set; }
        public Guid SourceId { get; set; }
        public DataSchemaEntity? Source { get; set; }
        public Guid TargetId { get; set; }
        public DataSchemaEntity? Target { get; set; }
    }
}
