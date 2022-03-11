using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task
{
    public class ServiceTaskDetailDTO : ServiceTaskDetailPartialDTO
    {
        public List<TaskAllDTO> OtherTasks { get; set; } = new List<TaskAllDTO>();
        public TaskAllDTO SelectedTask { get; set; } = new TaskAllDTO();        
    }
}
