using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Interfaces;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BPMS_DTOs.Account
{
    public class AddressDTO : IAuthInfo
    {
        public string URL { get; set; } = StaticData.ThisSystemURL;

        [JsonIgnore]
        public byte[]? Key { get; set; }

        [JsonIgnore]
        public Guid SystemId { get; set; }
        public Guid Salt { get; set; } = Guid.NewGuid();
    }
}