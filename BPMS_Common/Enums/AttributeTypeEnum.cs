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
        Selection,
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
                
                case AttributeTypeEnum.Selection:
                    return "výběr";

                default:
                    return "";
            }
        }
    }
}
