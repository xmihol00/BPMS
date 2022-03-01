using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum WorkflowStateEnum
    {
        Waiting,
        Active,
        Paused,
        Finished,
        Canceled
    }

    public static class WorkflowState
    {
        public static string ToLabel(this WorkflowStateEnum state)
        {
            switch (state)
            {
                case WorkflowStateEnum.Waiting:
                    return "čekající na spuštění";
                
                case WorkflowStateEnum.Active:
                    return "běžící";
                
                case WorkflowStateEnum.Paused:
                    return "pozastavené";
                
                case WorkflowStateEnum.Canceled:
                    return "zrušené";
                
                case WorkflowStateEnum.Finished:
                    return "dokončené";

                default:
                    return "";
            }
        }
    }
}
