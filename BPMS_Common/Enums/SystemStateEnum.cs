using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum SystemStateEnum
    {
        Inactive,
        Reactivated,
        Waiting,
        Active,
        Deactivated,
        ThisSystem
    }

    public static class SystemState
    {
        public static string ToLabel(this SystemStateEnum value)
        {
            switch (value)
            {
                default:
                    return "";

                case SystemStateEnum.Inactive:
                    return "žádost o spojení";
                
                case SystemStateEnum.Reactivated:
                    return "obnovení spojení";
                
                case SystemStateEnum.Waiting:
                    return "spojení zažádáno";
                
                case SystemStateEnum.Active:
                    return "aktivní spojení";
                
                case SystemStateEnum.Deactivated:
                    return "spojení deaktivováno";
                
                case SystemStateEnum.ThisSystem:
                    return "tento systém";
            }
        }
    }
}
