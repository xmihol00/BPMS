using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.Lane;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Sharing
{
    public class ModelDetailShare
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SVG { get; set; } = string.Empty;
        public string SenderURL { get; set; } = string.Empty;
        public ModelStateEnum State { get; set; }
        public IEnumerable<PoolShareDTO> Pools { get; set; } = new List<PoolShareDTO>();
        public IEnumerable<LaneEntity> Lanes { get; set; } = new List<LaneEntity>();
        public IEnumerable<UserTaskModelEntity> UserTasks { get; set; } = new List<UserTaskModelEntity>();
        public IEnumerable<ServiceTaskModelEntity> ServiceTasks { get; set; } = new List<ServiceTaskModelEntity>();
        public IEnumerable<StartEventModelEntity> StartEvents { get; set; } = new List<StartEventModelEntity>();
        public IEnumerable<EndEventModelEntity> EndEvents { get; set; } = new List<EndEventModelEntity>();
        public IEnumerable<SendMessageEventModelEntity> SendMessageEvents { get; set; } = new List<SendMessageEventModelEntity>();
        public IEnumerable<RecieveMessageEventModelEntity> RecieveMessageEvents { get; set; } = new List<RecieveMessageEventModelEntity>();
        public IEnumerable<SendSignalEventModelEntity> SendSignalEvents { get; set; } = new List<SendSignalEventModelEntity>();
        public IEnumerable<RecieveSignalEventModelEntity> RecieveSignalEvents { get; set; } = new List<RecieveSignalEventModelEntity>();
        public IEnumerable<ParallelGatewayModelEntity> ParallelGateways { get; set; } = new List<ParallelGatewayModelEntity>();
        public IEnumerable<ExclusiveGatewayModelEntity> ExclusiveGateways { get; set; } = new List<ExclusiveGatewayModelEntity>();
        public IEnumerable<FlowEntity> Flows { get; set; } = new List<FlowEntity>();
    }
}
