using BPMS_Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public DateTime Date { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public NotificationStateEnum State { get; set; }
        public string? Info { get; set; }
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
    }
}
