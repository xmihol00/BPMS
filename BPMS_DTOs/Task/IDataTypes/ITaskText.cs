using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task.IDataTypes
{
    public interface ITaskText : ITaskData
    {
        public string? Value { get; set; }
    }
}
