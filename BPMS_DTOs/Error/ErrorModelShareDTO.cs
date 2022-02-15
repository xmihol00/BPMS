using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Error
{
    public class ErrorModelShareDTO
    {
        public string Url { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
    }
}
