using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class SystemEntity
    {
        public Guid Id { get; set; }
        public string UniqueName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ObtainedName { get; set; } = string.Empty;
        public string ObtainedKey { get; set; } = string.Empty;
        public List<SystemAgendaEntity> Agendas { get; set; } = new List<SystemAgendaEntity>();
        public List<SystemPoolEntity> Pools { get; set; } = new List<SystemPoolEntity>();
    }
}
