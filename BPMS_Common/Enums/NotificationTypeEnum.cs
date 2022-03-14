using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum NotificationTypeEnum
    {
        MissedTask,
        NewTask,
        NewRole,
        NewWorkflow,
        NewAgenda,
        RemovedRole,
        NewModel,
        NewSystem,
        DeactivatedSystem,
        ReactivateSystem,
    }

    public static class NotificationType
    {
        public static string ToText(this NotificationTypeEnum value)
        {
            switch (value)
            {
                default:
                    return "";
                
                case NotificationTypeEnum.MissedTask:
                    return "Máte promeškaný úkol";

                case NotificationTypeEnum.NewTask:
                    return "Máte nový úkol";

                case NotificationTypeEnum.NewRole:
                    return "Byla Vám přiřazena role v agendě";

                case NotificationTypeEnum.RemovedRole:
                    return "Byla Vám odebrána role v agendě";

                case NotificationTypeEnum.NewWorkflow:
                    return "Bylo Vám přiřazeno pod správu workflow";
                
                case NotificationTypeEnum.NewAgenda:
                    return "Byla Vám přiřazena pod správu agenda";
                
                case NotificationTypeEnum.NewModel:
                    return "Do agendy pod Vaší správou byl sdílen model";
                
                case NotificationTypeEnum.NewSystem:
                    return "Byla přijata žádost o vytvoření systému";
                
                case NotificationTypeEnum.DeactivatedSystem:
                    return "Byla deaktivován systém";
                
                case NotificationTypeEnum.ReactivateSystem:
                    return "Byla přijata žádost o reaktivování systému";
            }
        }

        public static string ToHref(this NotificationTypeEnum value)
        {
            switch (value)
            {
                default:
                    return "";
                
                case NotificationTypeEnum.MissedTask:
                case NotificationTypeEnum.NewTask:
                    return "/Task/UserDetail/";

                case NotificationTypeEnum.NewRole:
                case NotificationTypeEnum.NewAgenda:
                case NotificationTypeEnum.RemovedRole:
                    return "/Agenda/Detail/";

                case NotificationTypeEnum.NewWorkflow:
                    return "/Workflow/Detail/";
                
                case NotificationTypeEnum.NewModel:
                    return "/Model/Detail/";
                
                case NotificationTypeEnum.NewSystem:
                case NotificationTypeEnum.DeactivatedSystem:
                case NotificationTypeEnum.ReactivateSystem:
                    return "/System/Detail/";
            }
        }
    }
}
