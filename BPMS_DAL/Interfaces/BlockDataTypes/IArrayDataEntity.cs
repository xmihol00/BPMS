using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Interfaces.BlockDataTypes
{
    public interface IArrayDataEntity : ITaskDataEntity
    {
        public DataTypeEnum Type { get; set; }
    }
}
