using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ServiceDataSchemaEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string? StaticData { get; set; } = string.Empty;
        public uint Order { get; set; } 
        public bool Compulsory { get; set; }
        public DataTypeEnum Type { get; set; }
        public DirectionEnum Direction { get; set; }
        public Guid? ParentId { get; set; }
        public ServiceDataSchemaEntity? Parent { get; set; }
        public List<ServiceDataSchemaEntity> Children { get; set; } = new List<ServiceDataSchemaEntity>();
        public Guid ServiceId { get; set; }
        public ServiceEntity? Service { get; set; }
        public List<ConditionDataEntity> Conditions { get; set; } = new List<ConditionDataEntity>();
        public List<TaskDataEntity> Data { get; set; } = new List<TaskDataEntity>();
    }
}
