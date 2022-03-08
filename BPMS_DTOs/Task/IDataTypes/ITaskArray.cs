using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task.IDataTypes
{
    public interface ITaskArray : ITaskData
    {
        public List<TaskDataDTO> Values { get; set; }
        public DataTypeEnum Type { get; set; }
    }
}
