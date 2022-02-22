using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum AttributeTypeEnum
    {
        String,
        Number,
        Text,
        File,
        YesNo,
        Select,
        Date
    }

    public static class AttributeType
    {
        public static string ToLabel(this AttributeTypeEnum value)
        {
            switch (value)
            {
                case AttributeTypeEnum.String:
                    return "řetězec";
                
                case AttributeTypeEnum.Number:
                    return "číslo";
                
                case AttributeTypeEnum.Text:
                    return "text";
                
                case AttributeTypeEnum.File:
                    return "soubor";
                
                case AttributeTypeEnum.YesNo:
                    return "ano/ne";
                
                case AttributeTypeEnum.Select:
                    return "výběr";
                
                case AttributeTypeEnum.Date:
                    return "datum";

                default:
                    return "";
            }
        }
    }
}
