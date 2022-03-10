using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace BPMS_DAL.Sharing
{
    public class DataSchemaDataMap
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public List<TaskDataEntity> Data { get; set; } = new List<TaskDataEntity>();
    }
}
