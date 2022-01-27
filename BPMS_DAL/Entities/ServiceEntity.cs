﻿using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ServiceEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ServiceTypeEnum ServiceType { get; set; }
        public SerializationEnum Serialization { get; set; }
        public HttpMethodEnum HttpMethod { get; set; }
        public string URL { get; set; } = string.Empty;
        public List<ServiceTaskModelEntity> ServiceTasks { get; set; } = new List<ServiceTaskModelEntity>();
    }
}
