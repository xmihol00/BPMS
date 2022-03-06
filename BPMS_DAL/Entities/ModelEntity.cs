using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class ModelEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SVG { get; set; } = string.Empty;
        public ModelStateEnum State { get; set; }
        public Guid? AgendaId { get; set; }

        [JsonIgnore]
        public AgendaEntity? Agenda { get; set; }
        public List<PoolEntity> Pools { get; set; } = new List<PoolEntity>();

        [JsonIgnore]
        public List<WorkflowEntity> Workflows { get; set; } = new List<WorkflowEntity>();
    }
}
