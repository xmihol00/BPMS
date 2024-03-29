﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using Newtonsoft.Json;

namespace BPMS_DAL.Entities
{
    public class BlockModelEntity : IBlockModelEntity
    {
        public BlockModelEntity() {}
        public BlockModelEntity(PoolEntity pool)
        {
            PoolId = pool.Id;
            Pool = pool;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public uint Order { get; set; }
        public Guid PoolId { get; set; }
        public Guid LaneId { get; set; }

        [JsonIgnore]
        public PoolEntity? Pool { get; set; }

        [JsonIgnore]
        public LaneEntity? Lane { get; set; }


        [JsonIgnore]
        public List<FlowEntity> InFlows { get; set; } = new List<FlowEntity>();
        
        [JsonIgnore]
        public List<FlowEntity> OutFlows { get; set; } = new List<FlowEntity>();

        [JsonIgnore]
        public List<BlockWorkflowEntity> BlockWorkflows { get; set; } = new List<BlockWorkflowEntity>();

        [JsonIgnore]
        public List<AttributeEntity> Attributes { get; set; } = new List<AttributeEntity>();

        [JsonIgnore]
        public List<AttributeMapEntity> MappedAttributes { get; set; } = new List<AttributeMapEntity>();

        [JsonIgnore]
        public List<BlockModelDataSchemaEntity> DataSchemas { get; set; } = new List<BlockModelDataSchemaEntity>();
    }
}
