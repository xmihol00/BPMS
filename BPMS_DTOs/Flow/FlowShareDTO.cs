using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Flow
{
    public class FlowShareDTO
    {
        public Guid InBlockId { get; set; }
        public Guid OutBlockId { get; set; }
    }
}
