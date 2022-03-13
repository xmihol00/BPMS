using BPMS_Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ConnectionRequestEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public Guid ForeignUserId { get; set; }
        public Guid SystemId { get; set; }
        public SystemEntity? System { get; set; }
    }
}
