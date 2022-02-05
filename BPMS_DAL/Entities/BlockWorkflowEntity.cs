﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class BlockWorkflowEntity
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public DateTime SolvedDate { get; set; }
        public Guid WorkflowId { get; set; }
        public WorkflowEntity? Workflow { get; set; }
        public Guid BlockModelId { get; set; }
        public BlockModelEntity? BlockModel { get; set; }
        public List<TaskDataEntity> TaskData { get; set; } = new List<TaskDataEntity>();
    }
}
