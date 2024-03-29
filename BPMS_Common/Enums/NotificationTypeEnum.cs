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
        NewUserTask,
        NewRole,
        NewWorkflow,
        NewAgenda,
        RemovedRole,
        NewModel,
        NewSystem,
        DeactivatedSystem,
        ReactivateSystem,
        ActivatedSystem,
        ModelRun,
        NewServiceTask,
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

                case NotificationTypeEnum.NewUserTask:
                case NotificationTypeEnum.NewServiceTask:
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
                    return "Byl deaktivován systém";
                
                case NotificationTypeEnum.ReactivateSystem:
                    return "Byla přijata žádost o reaktivování systému";
                
                case NotificationTypeEnum.ActivatedSystem:
                    return "Byl aktivován systém";
                
                case NotificationTypeEnum.ModelRun:
                    return "Žádost o spuštění modelu";
            }
        }

        public static string ToHref(this NotificationTypeEnum value)
        {
            switch (value)
            {
                default:
                    return "";
                
                case NotificationTypeEnum.MissedTask:
                case NotificationTypeEnum.NewUserTask:
                    return "/Task/UserDetail/";
                
                case NotificationTypeEnum.NewServiceTask:
                    return "/Task/ServiceDetail/";

                case NotificationTypeEnum.NewRole:
                case NotificationTypeEnum.NewAgenda:
                case NotificationTypeEnum.RemovedRole:
                    return "/Agenda/Detail/";

                case NotificationTypeEnum.NewWorkflow:
                    return "/Workflow/Detail/";
                
                case NotificationTypeEnum.NewModel:
                case NotificationTypeEnum.ModelRun:
                    return "/Model/Detail/";
                
                case NotificationTypeEnum.NewSystem:
                case NotificationTypeEnum.DeactivatedSystem:
                case NotificationTypeEnum.ReactivateSystem:
                case NotificationTypeEnum.ActivatedSystem:
                    return "/System/Detail/";
            }
        }
    }
}
