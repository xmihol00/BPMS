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
                    return "Byla Vám přiřazena role";

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
                    return "/Agenda/Detail/";

            }
        }

        public static string ToButton(this NotificationTypeEnum value)
        {
            switch (value)
            {
                default:
                    return "";
                
                case NotificationTypeEnum.MissedTask:
                case NotificationTypeEnum.NewTask:
                    return "Vyřešit";

                case NotificationTypeEnum.NewRole:
                    return "Navštívit";
            }
        }
    }
}
