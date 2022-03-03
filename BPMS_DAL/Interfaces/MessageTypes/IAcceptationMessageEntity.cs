using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities.MessageTypes
{
    public interface IAcceptationMessageEntity : IMessageEntity
    {
        public string? Text { get; set; }
        public string FullUserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
    }
}
