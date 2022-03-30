using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;

namespace BPMS_Common.Interfaces
{
    public interface IAuth
    {
        public byte[]? Key { get; set; }
        public Guid SystemId { get; set; }
        public Guid MessageId { get; set; }
        public EncryptionLevelEnum Encryption { get; set; }
        public byte[]? PayloadHash { get; set; }
        public byte[]? PayloadKey { get; set; }
        public byte[]? PayloadIV { get; set; }
    }
}
