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
                    return "odložitelný";
                
                case TaskPriorityEnum.Medium:
                    return "běžný";
                
                case TaskPriorityEnum.High:
                    return "nalehavý";

                case TaskPriorityEnum.Urgent:
                    return "urgentní";
                
                default:
                    return "";
            }
        }
    }
}
