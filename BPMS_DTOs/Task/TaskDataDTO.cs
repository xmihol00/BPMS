using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;
using BPMS_DTOs.Task.IDataTypes;

namespace BPMS_DTOs.Task
{
    public class TaskDataDTO : ITaskData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Compulsory { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
