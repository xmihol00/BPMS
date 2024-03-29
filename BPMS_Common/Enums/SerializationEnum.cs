﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum SerializationEnum
    {
        JSON,
        XMLMarks,
        XMLAttributes,
        URL,
        Replace
    }

    public static class Serialization
    {
        public static string ToLabel(this SerializationEnum value)
        {
            switch (value)
            {
                case SerializationEnum.JSON:
                    return "JSON";
                
                case SerializationEnum.XMLMarks:
                    return "XML (značky)";

                case SerializationEnum.XMLAttributes:
                    return "XML (atributy)";
                
                case SerializationEnum.URL:
                    return "URL query";
                
                case SerializationEnum.Replace:
                    return "nahradit v URL";

                default:
                    return "";
            }
        }

        public static string ToMIME(this SerializationEnum value)
        {
            switch (value)
            {
                case SerializationEnum.JSON:
                    return "application/json";
                
                case SerializationEnum.XMLMarks:
                case SerializationEnum.XMLAttributes:
                    return "text/xml";
                
                case SerializationEnum.URL:
                    return "application/x-www-form-urlencoded";
                
                default:
                    return "";
            }
        }
    }
}
