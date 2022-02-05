using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class ArrayBlockEntity : TaskDataEntity
    {
        public DataTypeEnum Type { get; set; }
    }
}
