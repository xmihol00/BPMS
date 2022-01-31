using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ModelEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string SVG { get; set; } = string.Empty;
        public Guid? AgendaId { get; set; }
        public AgendaEntity? Agenda { get; set; }
        public List<PoolEntity> Pools { get; set; } = new List<PoolEntity>();
        public List<WorkflowEntity> Workflows { get; set; } = new List<WorkflowEntity>();
    }
}
