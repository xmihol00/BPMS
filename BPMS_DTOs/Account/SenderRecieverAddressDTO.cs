using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SenderRecieverAddressDTO : AddressDTO, IAddressAuth
    {
        [JsonIgnore]
        public Guid ForeignBlockId { get; set; }
        
        [JsonIgnore]
        public string DestinationURL { get; set; } = string.Empty;

        [JsonIgnore]
        public string SystemName { get; set; } = string.Empty;
    }
}
