using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public interface IMessageEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public MessageTypeEnum Type { get; set; }
        public Guid SystemId { get; set; }
        public SystemEntity System { get; set; } 
    }
}
