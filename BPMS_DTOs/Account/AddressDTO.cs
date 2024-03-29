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
    public class AddressDTO : IAuth
    {
        public AddressDTO(Guid? messageId = null) 
        {
            if (messageId == null)
            {
                MessageId = Guid.NewGuid();
            }
            else
            {
                MessageId = messageId.Value;
            }
        }

        [JsonIgnore]
        public byte[]? Key { get; set; }

        [JsonIgnore]
        public Guid SystemId { get; set; }
        public Guid MessageId { get; set; }
        
        [JsonIgnore]
        public EncryptionLevelEnum Encryption { get; set; }
        public byte[]? PayloadHash { get; set; }
        public byte[]? PayloadKey { get; set; }
        public byte[]? PayloadIV { get; set; }
    }
}
