﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Interfaces.WorkflowBlocks;

namespace BPMS_DAL.Entities.WorkflowBlocks
{
    public class UserTaskWorkflowEntity : TaskWorkflowEntity, IUserTaskWorkflowEntity
    {
        public DateTime SolveDate { get; set; }
        public TaskPriorityEnum Priority { get; set; }
    }
}
