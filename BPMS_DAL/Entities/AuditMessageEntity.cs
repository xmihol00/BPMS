using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class AuditMessageEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid SystemId { get; set; }
        public SystemEntity System { get; set; } = new SystemEntity();
    }
}
