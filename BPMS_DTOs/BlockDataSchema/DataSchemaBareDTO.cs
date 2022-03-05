using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.DataSchema
{
    public class DataSchemaBareDTO
    {
        public Guid Id { get; set; }
        public DataTypeEnum Type { get; set; }
    }
}
