using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum ModelStateEnum
    {
        New,
        Shared,
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
                case ModelStateEnum.New:
                    return "nový";
                
                case ModelStateEnum.Shareable:
                    return "možný sdílet";

                case ModelStateEnum.Incorrect:
                    return "konfigurace";

                case ModelStateEnum.Executable:
                    return "sputitelný";

                case ModelStateEnum.Shared:
                    return "vysdílený";

                case ModelStateEnum.Waiting:
                    return "čekající";

                default:
                    return "";
            }
        }
    }
}
