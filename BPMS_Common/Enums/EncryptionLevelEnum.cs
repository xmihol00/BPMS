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
        Audit,
        Hash,
        Encrypted,
    }

    public static class EncryptionLevel
    {
        public static string ToLabel(this EncryptionLevelEnum value)
        {
            switch (value)
            {
                case EncryptionLevelEnum.Auth:
                    return "Autentizace";
                
                case EncryptionLevelEnum.Audit:
                    return "Autentizace + Auditní zprávy";
                
                case EncryptionLevelEnum.Hash:
                    return "Autentizace + Auditní zprávy + Hash";
                
                case EncryptionLevelEnum.Encrypted:
                    return "Autentizace + Auditní zprávy + Hash + Šifrování";
                
                default:
                    return "";
            }
        }
    }
}
