using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task.IDataTypes
{
    public interface ITaskFile : ITaskData
    {
        public string? FileName { get; set; }
        public string FileFormats { get; set; }
    }
}
