using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum BlockWorkflowStateEnum
    {
        NotStarted,
        Active,
        Solved,
        SolvedByUser,
        Paused,
        Canceled,
    }

    public static class BlockWorkflowState
    {
        public static string ToLabel(this BlockWorkflowStateEnum value)
        {
            switch (value)
            {
                default:
                    return "";

                case BlockWorkflowStateEnum.NotStarted:
                    return "Nezapočnutý";
                    
                case BlockWorkflowStateEnum.Active:
                    return "Aktivní";
                    
                case BlockWorkflowStateEnum.Solved:
                case BlockWorkflowStateEnum.SolvedByUser:
                    return "Vyřešený";
                    
                case BlockWorkflowStateEnum.Paused:
                    return "Pozastavený";
                    
                case BlockWorkflowStateEnum.Canceled:
                    return "Zrušený";
                    
            }
        }
    }
}
