using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.User;
using Newtonsoft.Json;
using BPMS_DAL.Sharing;
using BPMS_Common.Enums;
using BPMS_DAL;
using BPMS_DTOs.Account;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Agenda;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DTOs.Lane;
using BPMS_DTOs.Role;

namespace BPMS_BL.Facades
{
    public class ModelFacade : BaseFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly UserRepository _userRepository;
        private readonly FlowRepository _flowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly PoolRepository _poolRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly AttributeRepository _attributeRepository;
        private readonly DataSchemaRepository _dataSchemaRepository;
        private readonly LaneRepository _laneRepository;
        private readonly BpmsDbContext _context;
        private readonly IMapper _mapper;

        public ModelFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                           BlockModelRepository blockModelRepository, PoolRepository poolRepository, WorkflowRepository workflowRepository,
                           AgendaRoleRepository agendaRoleRepository, AttributeRepository attributeRepository, 
                           FilterRepository filterRepository, DataSchemaRepository dataSchemaRepository, LaneRepository laneRepository, 
                           BpmsDbContext context, IMapper mapper)
        : base(filterRepository)
        {
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _flowRepository = flowRepository;
            _blockModelRepository = blockModelRepository;
            _poolRepository = poolRepository;
            _workflowRepository = workflowRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _attributeRepository = attributeRepository;
            _dataSchemaRepository = dataSchemaRepository;
            _laneRepository = laneRepository;
            _context = context;
            _mapper = mapper;
        }

        public void SetFilters(bool[] filters)
        {
            _modelRepository.Filters = filters;
            _modelRepository.UserId = UserId;
            _userRepository.UserId = UserId;
        }

        public async Task<List<ModelAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, UserId);
            _modelRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _modelRepository.All();
        }

        public async Task Remove(Guid id)
        {
            ModelEntity entity = await _modelRepository.DetailDeep(id);
            _modelRepository.Remove(entity);
            await _modelRepository.Save();
        }

        public async Task<ModelDetailDTO> Detail(Guid id)
        {
            ModelDetailDTO detail = await _modelRepository.Detail(id);
            detail.OtherModels = await _modelRepository.All(id);
            detail.SelectedModel = await _modelRepository.Selected(id);

            return detail;
        }

        public async Task<ModelDetailPartialDTO> DetailPartial(Guid id)
        {
            return await _modelRepository.Detail(id);
        }

        public async Task<ModelInfoCardDTO> Edit(ModelEditDTO dto)
        {
            ModelEntity entity = await _modelRepository.BareAgenda(dto.Id);
            entity.Name = dto.Name;
            entity.Description = dto.Description ?? "";
            await _modelRepository.Save();

            return new ModelInfoCardDTO
            {
                AgendaId = entity.AgendaId,
                AgendaName = entity.Agenda?.Name,
                Description = entity.Description,
                Id = entity.Id,
                Name = entity.Name,
                State = entity.State,
                SelectedModel = await _modelRepository.Selected(entity.Id)
            };
        }

        public async Task<ModelDetailDTO> Share(Guid id)
        {
            ModelDetailShare model = await _modelRepository.Share(id);
            model.Flows = await _flowRepository.Share(id);

            IEnumerable<IGrouping<Type, BlockModelEntity>> allBlocks = await _blockModelRepository.ShareBlocks(id);
            model.EndEvents = GetCorrectBlocks<EndEventModelEntity>(allBlocks);
            model.StartEvents = GetCorrectBlocks<StartEventModelEntity>(allBlocks);
            model.UserTasks = GetCorrectBlocks<UserTaskModelEntity>(allBlocks);
            model.ServiceTasks = GetCorrectBlocks<ServiceTaskModelEntity>(allBlocks);
            model.ParallelGateways = GetCorrectBlocks<ParallelGatewayModelEntity>(allBlocks);
            model.ExclusiveGateways = GetCorrectBlocks<ExclusiveGatewayModelEntity>(allBlocks);
            model.SendMessageEvents = GetCorrectBlocks<SendMessageEventModelEntity>(allBlocks);
            model.RecieveMessageEvents = GetCorrectBlocks<RecieveMessageEventModelEntity>(allBlocks);
            model.SendSignalEvents = GetCorrectBlocks<SendSignalEventModelEntity>(allBlocks);
            model.RecieveSignalEvents = GetCorrectBlocks<RecieveSignalEventModelEntity>(allBlocks);

            string serilizedModel = JsonConvert.SerializeObject(model);
            
            bool shared = true;
            foreach (DstAddressDTO authAddress in await _poolRepository.Addresses(id))
            {
                shared &= await CommunicationHelper.ShareModel(authAddress, serilizedModel);
            }

            if (shared)
            {
                _modelRepository.ChangeState(model.Id, ModelStateEnum.Executable);
            }
            await _modelRepository.Save();

            ModelDetailDTO detail = await _modelRepository.DetailNoWF(id);
            detail.SelectedModel = await _modelRepository.Selected(id);
            return detail;
        }

        public async Task LaneEdit(LaneEditDTO dto)
        {
            LaneEntity entity = await _laneRepository.Bare(dto.Id);
            entity.RoleId = dto.RoleId;
            await _laneRepository.Save();
        }

        public async Task<LaneConfigDTO> LaneConfig(Guid id)
        {
            LaneConfigDTO lane = await _laneRepository.Config(id);
            lane.Roles.Add(new RoleAllDTO
            {
                Id = null,
                Name = "Nevybrána"
            });
            return lane;
        }

        public Task<List<UserIdNameDTO>> Run(Guid id)
        {
            return _modelRepository.WorflowKeepers(id);
        }

        public async Task<(ModelDetailDTO?, Guid)> Run(ModelRunDTO dto)
        {
            WorkflowEntity? workflow = await _workflowRepository.WaitingOrDefault(dto.Id);
            if (workflow == null)
            {
                workflow = new WorkflowEntity()
                {
                    ModelId = dto.Id,
                    State = WorkflowStateEnum.Waiting,
                    Description = dto.Description ?? "",
                    Name = dto.Name,
                    AgendaId = await _modelRepository.AgendaId(dto.Id) ?? Guid.Empty,
                    AdministratorId = dto.UserId,
                    Start = DateTime.Now
                };

                await _workflowRepository.Create(workflow);
            }
            else
            {
                workflow.Description = dto.Description ?? "";
                workflow.Name = dto.Name;
                workflow.AdministratorId = dto.UserId;
                workflow.Start = DateTime.Now;
                _workflowRepository.Update(workflow);
            }
            await _workflowRepository.Save();

            string checkMessage = JsonConvert.SerializeObject(new WorkflowShare 
            { 
                ModelId = dto.Id,
                Workflow = workflow
            });
            string runMessage = JsonConvert.SerializeObject(new ModelIdWorkflowDTO
            {
                ModelId = dto.Id,
                WorkflowId = workflow.Id
            });

            bool run = true;
            List<DstAddressDTO> pools = await _poolRepository.Addresses(dto.Id);
            foreach (DstAddressDTO pool in pools)
            {
                run &= await CommunicationHelper.IsModelRunable(pool, checkMessage);
            }

            if (run)
            {
                foreach (DstAddressDTO pool in pools)
                {
                    pool.MessageId = Guid.NewGuid();
                    run &= await CommunicationHelper.RunModel(pool, runMessage);
                }   
            }

            if (run)
            {
                WorkflowHelper workflowHelper = new WorkflowHelper(_context, UserId);
                await workflowHelper.CreateWorkflow(dto.Id, workflow);
            }
            else
            {
                _modelRepository.ChangeState(dto.Id, ModelStateEnum.Waiting);
            }
            await _modelRepository.Save();
            
            if (!run)
            {
                ModelDetailDTO detail = await _modelRepository.DetailNoWF(dto.Id);
                detail.SelectedModel = await _modelRepository.Selected(dto.Id);
                return (detail, workflow.Id);
            }

            return (null, workflow.Id);
        }

        public Task<ModelDetailHeaderDTO> Header(Guid id)
        {
            return _modelRepository.Header(id);
        }

        private IEnumerable<T> GetCorrectBlocks<T>(IEnumerable<IGrouping<Type, BlockModelEntity>> allBlocks) where T: BlockModelEntity
        {
            return allBlocks.Where(x => x.Key == typeof(T)).SelectMany(x => x).Cast<T>();
        }

        public async Task<ModelOverviewDTO> Overview()
        {
            return new ModelOverviewDTO()
            {
                Models = await _modelRepository.All()
            };
        }
    }
}
