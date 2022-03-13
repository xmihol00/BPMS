using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum DataTypeEnum
    {
        String,
        Number,
        Bool,
        Object,
        ArrayString,
        ArrayNumber,
        ArrayBool,
        //ArrayObject,
        //ArrayArray,
    }

    public static class DataType
    {
        public static string ToLabel(this DataTypeEnum value)
        {
            switch (value)
            {
                case DataTypeEnum.Object:
                    return "objekt";
                
                case DataTypeEnum.Bool:
                    return "booleovská hodnota";
                
                case DataTypeEnum.Number:
                    return "číslo";
                
                case DataTypeEnum.String:
                    return "řetězec";
                
                case DataTypeEnum.ArrayString:
                    return "pole řetězců";

                case DataTypeEnum.ArrayBool:
                    return "pole booleovských hodnota";

                case DataTypeEnum.ArrayNumber:
                    return "pole čísel";

                //case DataTypeEnum.ArrayObject:
                //    return "pole objektů";
                //
                //case DataTypeEnum.ArrayArray:
                //    return "pole polí";
                
                default:
                    return "";
            }
        }

        public static string ToHtmlType(this DataTypeEnum value)
        {
            switch (value)
            {
                case DataTypeEnum.Number:
                    return "number";
                
                case DataTypeEnum.String:
                    return "text";
                
                default:
                    return "";
            }
        }
    }
}
