using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum DataTypeEnum
    {
        Object,
        String,
        Number,
        Bool,
        Array
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
                
                case DataTypeEnum.Array:
                    return "pole";
                
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
