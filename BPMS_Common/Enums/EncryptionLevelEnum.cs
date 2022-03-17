using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum EncryptionLevelEnum
    {
        Auth,
        Hash,
        Encrypted
    }

    public static class EncryptionLevel
    {
        public static string ToLabel(this EncryptionLevelEnum value)
        {
            switch (value)
            {
                case EncryptionLevelEnum.Auth:
                    return "Autentizace";
                
                case EncryptionLevelEnum.Hash:
                    return "Autentizace + Hash";
                
                case EncryptionLevelEnum.Encrypted:
                    return "Autentizace + Hash + Šifrování";
                
                default:
                    return "";
            }
        }
    }
}
