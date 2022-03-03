using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;

namespace BPMS_DAL.Entities
{
    public class SystemEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public byte[]? Key { get; set; }
        public string URL { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SystemActivityEnum Activity { get; set; }
        public List<SystemAgendaEntity> Agendas { get; set; } = new List<SystemAgendaEntity>();
        public List<PoolEntity> Pools { get; set; } = new List<PoolEntity>();
        public List<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
    }
}
