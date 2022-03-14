using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.AuditMessage
{
    public class AuditMessageAllDTO
    {
        public DateTime Date { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
