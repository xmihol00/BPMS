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

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(BlockModelEntity)))
            {
                string name = typeof(ModelDetailShare).GetProperties()
                                                      .Where(x => x.PropertyType.IsGenericType)
                                                      .Select(x => x.PropertyType.GetGenericArguments().First())
                                                      .First(x => x == type)
                                                      .Name;
                
                dynamic value = typeof(ModelDetailShare).GetProperty(name).GetValue(dto);
                await _blockModelRepository.CreateRange(value);
            }

            await _modelRepository.Save();
            return "";
        }
    }
}
