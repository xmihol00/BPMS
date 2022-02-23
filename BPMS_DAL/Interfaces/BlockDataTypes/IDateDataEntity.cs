using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Interfaces.BlockDataTypes
{
    public interface IDateDataEntity : ITaskDataEntity
    {
        public DateTime? Value { get; set; }
    }
}
