using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum FilterTypeEnum
    {
        AgendaKeeper,
        AgendaRole,
        AgendaPausedWF,
        AgendaActiveWF,
        AgendaFinishedWF,
        AgendaCanceledWF,


        WorkflowKeeper,
        WorkflowPaused,
        WorkflowActive,
        WorkflowFinished,
        WorkflowCanceled,


        TaskActive,
        TaskSolved,
        TaskPaused,
        TaskCanceled,
    }

    public static class FilterType
    {
        public static string ToLabel(this FilterTypeEnum value)
        {
            switch (value)
            {
                default:
                    return "";
                
                case FilterTypeEnum.AgendaKeeper:
                    return "Správcuji";
                
                case FilterTypeEnum.AgendaRole:
                    return "Zastávám roli";
                
                case FilterTypeEnum.AgendaPausedWF:
                    return "Pozastavená workflow";
                
                case FilterTypeEnum.AgendaActiveWF:
                    return "Aktivní workflow";
                
                case FilterTypeEnum.AgendaFinishedWF:
                    return "Dokončená workflow";
                
                case FilterTypeEnum.AgendaCanceledWF:
                    return "Zrušená workflow";

                

                case FilterTypeEnum.WorkflowKeeper:
                    return "Správcuji";
                
                case FilterTypeEnum.WorkflowPaused:
                    return "Pozastavená";

                case FilterTypeEnum.WorkflowActive:
                    return "Aktivní";
                
                case FilterTypeEnum.WorkflowFinished:
                    return "Dokončená";
                
                case FilterTypeEnum.WorkflowCanceled:
                    return "Zrušená";

                
                
                case FilterTypeEnum.TaskPaused:
                    return "Pozastavené";

                case FilterTypeEnum.TaskActive:
                    return "Aktivní";
                
                case FilterTypeEnum.TaskSolved:
                    return "Vyřešené";
                
                case FilterTypeEnum.TaskCanceled:
                    return "Zrušené";
            }
        }
    }
}
