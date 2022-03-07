using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum ServiceAuthEnum
    {
        None,
        Basic,
        Bearer,
    }

    public static class ServiceAuth
    {
        public static string ToLabel(this ServiceAuthEnum value)
        {
            switch (value)
            {
                default:
                    return "";
                
                case ServiceAuthEnum.None:
                    return "Žádná";
                
                case ServiceAuthEnum.Basic:
                    return "Basic";
                
                case ServiceAuthEnum.Bearer:
                    return "Bearer";
            }
        }
    }
}
