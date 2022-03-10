using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Notification
{
    public class NotificationNewDTO
    {
        public Guid Id { get; set; }
        public string Href { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Info { get; set; }
        public string? State { get; set; }
    }
}
