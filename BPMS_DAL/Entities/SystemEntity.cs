using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class SystemEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public byte[]? Key { get; set; }
        public string URL { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SystemStateEnum State { get; set; }

        [JsonIgnore]
        public List<SystemAgendaEntity> Agendas { get; set; } = new List<SystemAgendaEntity>();

        [JsonIgnore]
        public List<PoolEntity> Pools { get; set; } = new List<PoolEntity>();

        [JsonIgnore]
        public List<AuditMessageEntity> AuditMessages { get; set; } = new List<AuditMessageEntity>();

        [JsonIgnore]
        public List<ForeignRecieveEventEntity> ForeignRecievers { get; set; } = new List<ForeignRecieveEventEntity>();

        [JsonIgnore]
        public List<ForeignSendEventEntity> ForeignSenedrs { get; set; } = new List<ForeignSendEventEntity>();

        public List<ConnectionRequestEntity> ConnectionRequests { get; set; } = new List<ConnectionRequestEntity>();
    }
}
