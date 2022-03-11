using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.BlockDataTypes;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class NumberDataEntity : TaskDataEntity, INumberDataEntity
    {
        public double? Value { get; set; }

        public override bool HasData()
        {
            return Value != null;
        }
    }
}
