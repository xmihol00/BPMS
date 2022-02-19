using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemUrlKeyDTO
    {
        public string URL { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}
