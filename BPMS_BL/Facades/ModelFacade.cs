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

namespace BPMS_BL.Facades
{
    public class ModelFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly UserRepository _userRepository;
        private readonly FlowRepository _flowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly PoolRepository _poolRepository;
        private readonly IMapper _mapper;

        public ModelFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                           BlockModelRepository blockModelRepository, PoolRepository poolRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _flowRepository = flowRepository;
            _blockModelRepository = blockModelRepository;
            _poolRepository = poolRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> Remove(Guid id)
        {
            ModelEntity entity = await _modelRepository.DetailDeep(id);
            _modelRepository.Remove(entity);
            await _modelRepository.Save();

            return entity.AgendaId;
        }

        public Task<ModelDetailDTO> Detail(Guid id)
        {
            return _modelRepository.Detail(id);
        }

        public async Task<ModelInfoDTO> Edit(ModelEditDTO dto)
        {
            ModelEntity entity = await _modelRepository.DetailRaw(dto.Id);
            entity.Name = dto.Name;
            entity.Description = dto.Description ?? "";

            await _modelRepository.Save();
            return new ModelInfoDTO 
            {
                Description = entity.Description,
                Name = entity.Name
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
            foreach (PoolDstAddressDTO pool in await _poolRepository.Addresses(id))
            {
                shared &= await CommunicationHelper.ShareModel(pool.DestinationURL, 
                                                               SymetricCypherHelper.JsonEncrypt(pool), 
                                                               serilizedModel);
            }

            if (shared)
            {
                _modelRepository.ChangeState(model.Id, ModelStateEnum.Shared);
            }
            await _modelRepository.Save();

            return "";
        }

        public async Task<bool> Run(Guid id)
        {
            List<PoolDstAddressDTO> pools = await _poolRepository.Addresses(id);
            string message = JsonConvert.SerializeObject(new ModelIdDTO { Id = id });

            bool run = true;
            foreach (PoolDstAddressDTO pool in pools)
            {
                run &= await CommunicationHelper.AskForModelRun(pool.DestinationURL, 
                                                                SymetricCypherHelper.JsonEncrypt(pool), 
                                                                message);
            }

            if (run)
            {
                foreach (PoolDstAddressDTO pool in pools)
                {
                    run &= await CommunicationHelper.RunModel(pool.DestinationURL, 
                                                              SymetricCypherHelper.JsonEncrypt(pool), 
                                                              message);
                }   
            }

            if (run)
            {
                _modelRepository.ChangeState(id, ModelStateEnum.Executable);

                // TODO create WF
            }
            else
            {
                _modelRepository.ChangeState(id, ModelStateEnum.Waiting);
            }
            await _modelRepository.Save();

            return run;
        }

        public Task<ModelHeaderDTO> Header(Guid id)
        {
            return _modelRepository.Header(id);
        }

        private IEnumerable<T> GetCorrectBlocks<T>(IEnumerable<IGrouping<Type, BlockModelEntity>> allBlocks) where T: BlockModelEntity
        {
            return allBlocks.Where(x => x.Key == typeof(T)).SelectMany(x => x).Cast<T>();
        }
    }
}
