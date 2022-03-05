using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.User;
using Newtonsoft.Json;
using BPMS_DAL.Sharing;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using BPMS_DAL;
using BPMS_DTOs.Account;

namespace BPMS_BL.Facades
{
    public class ModelFacade
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
        private readonly BpmsDbContext _context;
        private readonly IMapper _mapper;

        public ModelFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                           BlockModelRepository blockModelRepository, PoolRepository poolRepository, WorkflowRepository workflowRepository,
                           AgendaRoleRepository agendaRoleRepository, AttributeRepository attributeRepository, 
                           DataSchemaRepository dataSchemaRepository, BpmsDbContext context, IMapper mapper)
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
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid?> Remove(Guid id)
        {
            ModelEntity entity = await _modelRepository.DetailDeep(id);
            _modelRepository.Remove(entity);
            await _modelRepository.Save();

            return entity.AgendaId;
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
                Workflow = await _modelRepository.WaitingWorklfow(dto.Id),
                SelectedModel = await _modelRepository.Selected(entity.Id)
            };
        }

        public async Task<string> Share(Guid id)
        {
            ModelDetailShare model = await _modelRepository.Share(id);
            model.Pools = await _poolRepository.Share(id);
            model.Flows = await _flowRepository.Share(id);

            IEnumerable<IGrouping<Type, BlockModelEntity>> allBlocks = await _blockModelRepository.ShareBlocks(id);
            model.EndEvents = GetCorrectBlocks<EndEventModelEntity>(allBlocks);
            model.StartEvents = GetCorrectBlocks<StartEventModelEntity>(allBlocks);
            model.UserTasks = GetCorrectBlocks<UserTaskModelEntity>(allBlocks);
            model.ServiceTasks = GetCorrectBlocks<ServiceTaskModelEntity>(allBlocks);
            model.ParallelGateways = GetCorrectBlocks<ParallelGatewayModelEntity>(allBlocks);
            model.ExclusiveGateways = GetCorrectBlocks<ExclusiveGatewayModelEntity>(allBlocks);
            model.SendEvents = GetCorrectBlocks<SendEventModelEntity>(allBlocks);
            model.RecieveEvents = GetCorrectBlocks<RecieveEventModelEntity>(allBlocks);

            string serilizedModel = JsonConvert.SerializeObject(model);
            
            bool shared = true;
            foreach (DstAddressDTO pool in await _poolRepository.Addresses(id))
            {
                shared &= await CommunicationHelper.ShareModel(pool.DestinationURL, 
                                                               SymetricCipherHelper.JsonEncrypt(pool), 
                                                               serilizedModel);
            }

            if (shared)
            {
                _modelRepository.ChangeState(model.Id, ModelStateEnum.Shared);
            }
            await _modelRepository.Save();

            return "";
        }

        public Task<List<UserIdNameDTO>> Run(Guid id)
        {
            return _modelRepository.WorflowKeepers(id);
        }

        public async Task<bool> Run(ModelRunDTO dto)
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
                run &= await CommunicationHelper.IsModelRunable(pool.DestinationURL, 
                                                                SymetricCipherHelper.JsonEncrypt(pool), 
                                                                checkMessage);
            }

            if (run)
            {
                foreach (DstAddressDTO pool in pools)
                {
                    run &= await CommunicationHelper.RunModel(pool.DestinationURL, 
                                                              SymetricCipherHelper.JsonEncrypt(pool), 
                                                              runMessage);
                }   
            }

            if (run)
            {
                WorkflowHelper workflowHelper = new WorkflowHelper(_context);
                await workflowHelper.CreateWorkflow(dto.Id, workflow);
            }
            else
            {
                _modelRepository.ChangeState(dto.Id, ModelStateEnum.Waiting);
            }
            await _modelRepository.Save();

            return run;
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
