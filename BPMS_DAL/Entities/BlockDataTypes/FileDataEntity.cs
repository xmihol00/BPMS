using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class FileDataEntity : TaskDataEntity
    {
        public string Name { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
    }
}
