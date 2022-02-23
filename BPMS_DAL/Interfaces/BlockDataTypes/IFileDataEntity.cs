using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Interfaces.BlockDataTypes
{
    public interface IFileDataEntity : ITaskDataEntity
    {
        public string Name { get; set; }
        public string MIMEType { get; set; }
    }
}
