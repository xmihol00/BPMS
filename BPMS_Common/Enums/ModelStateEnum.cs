using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum ModelStateEnum
    {
        Incorrect,
        Shareable,
        Executable,
        Waiting,
    }

    public static class ModelState
    {
        public static string ToLabel(this ModelStateEnum value)
        {
            switch (value)
            {
                case ModelStateEnum.Incorrect:
                    return "nutný konfigurovat";
                
                case ModelStateEnum.Shareable:
                    return "možný sdílet";

                case ModelStateEnum.Executable:
                    return "sputitelný";

                case ModelStateEnum.Waiting:
                    return "čekající";

                default:
                    return "";
            }
        }
    }
}
