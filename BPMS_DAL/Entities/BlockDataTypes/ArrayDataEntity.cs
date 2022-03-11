using BPMS_Common.Enums;
using BPMS_DAL.Interfaces.BlockDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class ArrayDataEntity : TaskDataEntity, IArrayDataEntity
    {
        public DataTypeEnum Type { get; set; }

        public override bool HasData()
        {
            return true;
        }
    }
}
