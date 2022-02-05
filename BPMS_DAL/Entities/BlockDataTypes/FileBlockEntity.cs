using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class FileBlockEntity : TaskDataEntity
    {
        public string Name { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public Guid FileId { get; set; }
        public FileDataEntity? File { get; set; }
    }
}
