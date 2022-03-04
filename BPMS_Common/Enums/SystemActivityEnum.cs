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
        Waiting,
        Activated,
        Deactivated
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
                
                case SystemStateEnum.Waiting:
                    return "spojení zažádáno";
                
                case SystemStateEnum.Activated:
                    return "aktivní spojení";
                
                case SystemStateEnum.Deactivated:
                    return "spojení deaktivováno";
            }
        }
    }
}
