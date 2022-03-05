using BPMS_DAL.Entities.ModelBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ConditionDataEntity
    {
        public Guid ExclusiveGatewayId { get; set; }
        public Guid DataSchemaId { get; set; }
        public ExclusiveGatewayModelEntity? ExclusiveGateway { get; set; }
        public DataSchemaEntity? DataSchema { get; set; }
    }
}
