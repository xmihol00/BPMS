using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Sharing
{
    public class ModelDetailShare
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SVG { get; set; } = string.Empty;
        public ModelStateEnum State { get; set; }
        public IEnumerable<PoolShareDTO> Pools { get; set; } = new List<PoolShareDTO>();
        public IEnumerable<UserTaskModelEntity> UserTasks { get; set; } = new List<UserTaskModelEntity>();
        public IEnumerable<ServiceTaskModelEntity> ServiceTasks { get; set; } = new List<ServiceTaskModelEntity>();
        public IEnumerable<StartEventModelEntity> StartEvents { get; set; } = new List<StartEventModelEntity>();
        public IEnumerable<EndEventModelEntity> EndEvents { get; set; } = new List<EndEventModelEntity>();
        public IEnumerable<SendEventModelEntity> SendEvents { get; set; } = new List<SendEventModelEntity>();
        public IEnumerable<RecieveEventModelEntity> RecieveEventModelEntities { get; set; } = new List<RecieveEventModelEntity>();
        public IEnumerable<ParallelGatewayModelEntity> ParallelGateways { get; set; } = new List<ParallelGatewayModelEntity>();
        public IEnumerable<ExclusiveGatewayModelEntity> ExclusiveGateways { get; set; } = new List<ExclusiveGatewayModelEntity>();
        public IEnumerable<FlowEntity> Flows { get; set; } = new List<FlowEntity>();
    }
}
