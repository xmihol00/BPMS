using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.User;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class CommunicationFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly UserRepository _userRepository;
        private readonly FlowRepository _flowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly IMapper _mapper;

        public CommunicationFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                                   BlockModelRepository blockModelRepository, PoolRepository poolRepository, SystemRepository systemRepository, 
                                   IMapper mapper)
        {
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _flowRepository = flowRepository;
            _blockModelRepository = blockModelRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _mapper = mapper;
        }

        public async Task<string> ShareImport(ModelDetailShare dto, string auth)
        {
            await _modelRepository.Create(_mapper.Map<ModelEntity>(dto));

            foreach (PoolShareDTO poolDTO in dto.Pools)
            {
                PoolEntity poolEntity = _mapper.Map<PoolEntity>(poolDTO);
                if (poolDTO.SystemURL == StaticData.ThisSystemURL)
                {
                    poolEntity.SystemId = StaticData.ThisSystemId;
                }
                else
                {
                    poolEntity.SystemId = await _systemRepository.IdFromUrl(poolDTO.SystemURL);
                    if (poolEntity.SystemId == Guid.Empty)
                    {
                        poolEntity.SystemId = null;
                    }
                }
            }

            await _blockModelRepository.CreateRange(dto.EndEvents);
            await _blockModelRepository.CreateRange(dto.StartEvents);
            await _blockModelRepository.CreateRange(dto.UserTasks);
            await _blockModelRepository.CreateRange(dto.ServiceTasks);
            await _blockModelRepository.CreateRange(dto.ParallelGateways);
            await _blockModelRepository.CreateRange(dto.ExclusiveGateways);
            await _blockModelRepository.CreateRange(dto.SendEvents);
            await _blockModelRepository.CreateRange(dto.RecieveEvents);

            await _modelRepository.Save();
            return "";
        }
    }
}
