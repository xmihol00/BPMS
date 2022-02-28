using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.BlockDataTypes;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class FileDataEntity : TaskDataEntity, IFileDataEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
    }
}
