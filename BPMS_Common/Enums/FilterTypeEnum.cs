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


        ModelIncorrect,
        ModelSharable,
        ModelExecutable,
        ModelWaiting,


        ServiceREST,
        ServiceGET,
        ServicePOST,
        ServicePUT,
        ServicePATCH,
        ServiceDELETE,
        ServiceJSON,
        ServiceXML,
        ServiceURL,
        ServiceReplace,


        SystemInactive,
        SystemWaiting,
        SystemActive,
        SystemDeactivated,
        SystemThisSystem,


        UserAdmin,
        UserAgendaKeeper,
        UserModelKeeper,
        UserWorkflowKeeper,
        UserServiceKeeper,


        NotificationMarked,
        NotificationSeen,
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



                case FilterTypeEnum.ModelIncorrect:
                    return "Nutné konfigurovat";

                case FilterTypeEnum.ModelSharable:
                    return "Možné sdílet";
                
                case FilterTypeEnum.ModelExecutable:
                    return "Sputitelné";
                
                case FilterTypeEnum.ModelWaiting:
                    return "Čekající";


                case FilterTypeEnum.ServiceDELETE:
                    return "Metoda DELETE";

                case FilterTypeEnum.ServiceGET:
                    return "Metoda GET";
                
                case FilterTypeEnum.ServicePOST:
                    return "Metoda POST";
                
                case FilterTypeEnum.ServicePATCH:
                    return "Metoda PATCH";

                case FilterTypeEnum.ServicePUT:
                    return "Metoda PUT";

                case FilterTypeEnum.ServiceJSON:
                    return "Serializace JSON";
                
                case FilterTypeEnum.ServiceXML:
                    return "Serializace XML";
                
                case FilterTypeEnum.ServiceURL:
                    return "Serilizace do URL";
                
                case FilterTypeEnum.ServiceReplace:
                    return "Serilizace nahrazení URL";
                
                case FilterTypeEnum.ServiceREST:
                    return "Typ REST";


                
                case FilterTypeEnum.SystemInactive:
                    return "Žádající o spojení";

                case FilterTypeEnum.SystemWaiting:
                    return "Zažádáná spojení";

                case FilterTypeEnum.SystemActive:
                    return "Aktivní spojení";

                case FilterTypeEnum.SystemDeactivated:
                    return "Deaktivované spojení";

                case FilterTypeEnum.SystemThisSystem:
                    return "Tento systém";


                case FilterTypeEnum.UserAdmin:
                    return "Administrátoři";

                case FilterTypeEnum.UserAgendaKeeper:
                    return "Správci agend";

                case FilterTypeEnum.UserWorkflowKeeper:
                    return "Správci workflow";

                case FilterTypeEnum.UserServiceKeeper:
                    return "Správci služeb";
                
                case FilterTypeEnum.UserModelKeeper:
                    return "Správci modelů";
            }
        }
    }
}
