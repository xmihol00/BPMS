using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Filter
{
    public class FilterDTO
    {
        public FilterTypeEnum Filter { get; set; }
        public bool Removed { get; set; }
    }
}
