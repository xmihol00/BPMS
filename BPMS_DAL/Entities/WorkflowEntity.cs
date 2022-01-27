﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class WorkflowEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid AgendaId { get; set; }
        public AgendaEntity Agenda { get; set; } = new AgendaEntity();
        public Guid ModelId { get; set; }
        public ModelEntity Model { get; set; } = new ModelEntity();
        public List<BlockWorkflowEntity> Blocks { get; set; } = new List<BlockWorkflowEntity>();
    }
}
