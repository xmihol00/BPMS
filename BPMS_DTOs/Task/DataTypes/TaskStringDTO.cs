using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Task.IDataTypes;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task.DataTypes
{
    public class TaskStringDTO : TaskDataDTO, ITaskString
    {
        public string? Value { get; set; }
    }
}
