using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.BlockDataTypes;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class FileDataEntity : TaskDataEntity, IFileDataEntity
    {
        public string? FileName { get; set; }
        public string? MIMEType { get; set; }

        [NotMapped]
        public byte[]? Data { get; set; }

        public override bool HasData()
        {
            return FileName != null;
        }
    }
}
