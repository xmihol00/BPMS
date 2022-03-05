using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel
{
    public class SenderChangeDTO
    {
        public Guid Id { get; set; }
        public Guid SystemId { get; set; }
        public Guid BlockId { get; set; }
    }
}
