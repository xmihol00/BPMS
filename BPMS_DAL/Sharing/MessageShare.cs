using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Sharing
{
    public class MessageShare
    {
        public Guid? WorkflowId { get; set; }
        public Guid BlockId { get; set; }
        public IEnumerable<StringDataEntity> Strings { get; set; } = new List<StringDataEntity>(); 
        public IEnumerable<NumberDataEntity> Numbers { get; set; } = new List<NumberDataEntity>(); 
        public IEnumerable<TextDataEntity> Texts { get; set; } = new List<TextDataEntity>(); 
        public IEnumerable<DateDataEntity> Dates { get; set; } = new List<DateDataEntity>(); 
        public IEnumerable<BoolDataEntity> Bools { get; set; } = new List<BoolDataEntity>(); 
        public IEnumerable<FileDataEntity> Files { get; set; } = new List<FileDataEntity>();
        public IEnumerable<SelectDataEntity> Selects { get; set; } = new List<SelectDataEntity>();
        public IEnumerable<ArrayDataEntity> Arrays { get; set; } = new List<ArrayDataEntity>(); 
    }
}
