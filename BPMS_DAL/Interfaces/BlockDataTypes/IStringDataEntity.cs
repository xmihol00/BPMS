using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Interfaces.BlockDataTypes
{
    public interface IStringDataEntity : ITaskDataEntity
    {
        public string? Value { get; set; }
    }
}
