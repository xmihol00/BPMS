using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BPMS_DTOs.Pool
{
    public class PoolAddressDTO
    {
        public static string URL { get; } = StaticData.ThisSystemURL;
        public string Key { get; set; } = string.Empty;
    }
}
