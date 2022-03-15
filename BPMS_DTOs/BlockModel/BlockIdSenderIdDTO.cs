using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel
{
    public class BlockIdSenderIdDTO
    {
        public Guid BlockId { get; set; }
        public Guid SenderId { get; set; }
    }
}
