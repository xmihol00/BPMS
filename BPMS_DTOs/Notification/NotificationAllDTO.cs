using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Notification
{
    public class NotificationAllDTO
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public DateTime Date { get; set; }
        public string? Info { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public NotificationStateEnum State { get; set; }
    }
}
