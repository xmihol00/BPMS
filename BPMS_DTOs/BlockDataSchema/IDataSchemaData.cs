using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.DataSchema
{
    public interface IDataSchemaData : IDataSchema
    {
        public string? Data { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string? StaticData { get; set; }
        public string? Description { get; set; }
    }
}
