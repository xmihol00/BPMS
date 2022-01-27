using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class SystemAgendaEntity
    {
        public Guid AgendaId { get; set; }
        public Guid SystemId { get; set; }
        public SystemEntity System { get; set; } = new SystemEntity();
        public AgendaEntity Agenda { get; set; } = new AgendaEntity();
    }
}
