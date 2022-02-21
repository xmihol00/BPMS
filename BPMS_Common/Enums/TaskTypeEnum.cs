using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum TaskTypeEnum
    {
        UserTask,
        ServiceTask
    }

    public static class TaskType
    {
        public static string ToLabel(this TaskTypeEnum type)
        {
            switch (type)
            {
                case TaskTypeEnum.UserTask:
                    return "běžný úkol";
                
                case TaskTypeEnum.ServiceTask:
                    return "webová služba";
                
                default:
                    return "";
            }
        }
    }
}
