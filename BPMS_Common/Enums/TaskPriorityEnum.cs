using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum TaskPriorityEnum
    {
        Urgent,
        High,
        Medium,
        Low,
    }

    public static class TaskPriority
    {
        public static string ToLabel(this TaskPriorityEnum value)
        {
            switch (value)
            {
                case TaskPriorityEnum.Low:
                    return "odložitelná";
                
                case TaskPriorityEnum.Medium:
                    return "běžná";
                
                case TaskPriorityEnum.High:
                    return "nalehavá";

                case TaskPriorityEnum.Urgent:
                    return "urgentní";
                
                default:
                    return "";
            }
        }
    }
}
