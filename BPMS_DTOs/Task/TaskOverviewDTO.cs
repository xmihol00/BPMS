using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task
{
    public class TaskOverviewDTO
    {
        public List<TaskAllDTO> Tasks { get; set; } = new List<TaskAllDTO>();
    }
}
