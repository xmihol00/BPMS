using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class WorkflowEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkflowStateEnum State { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime ExpectedEnd { get; set; }
        public Guid AgendaId { get; set; }

        [JsonIgnore]
        public AgendaEntity? Agenda { get; set; }
        public Guid ModelId { get; set; }

        [JsonIgnore]
        public ModelEntity? Model { get; set; }
        public Guid? AdministratorId { get; set; }

        [JsonIgnore]
        public UserEntity? Administrator { get; set; }

        [JsonIgnore]
        public List<BlockWorkflowEntity> Blocks { get; set; } = new List<BlockWorkflowEntity>();
    }
}
