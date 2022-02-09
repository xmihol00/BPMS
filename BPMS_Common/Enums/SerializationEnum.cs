using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum SerializationEnum
    {
        JSON,
        XML,
        URL,
    }

    public static class Serialization
    {
        public static string ToLabel(this SerializationEnum value)
        {
            switch (value)
            {
                case SerializationEnum.JSON:
                    return "JSON";
                
                case SerializationEnum.XML:
                    return "XML";
                
                case SerializationEnum.URL:
                    return "Zakódování do URL";
                
                default:
                    return "";
            }
        }
    }
}
