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
            IEnumerable<IGrouping<Type, BlockModelEntity>> allBlocks = await _blockModelRepository.ShareBlocks(id);

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(BlockModelEntity)))
            {
                string name = typeof(ModelDetailShare).GetProperties()
                                                      .Where(x => x.PropertyType.IsGenericType)
                                                      .Select(x => x.PropertyType.GetGenericArguments().First())
                                                      .First(x => x == type)
                                                      .Name;
                
                typeof(ModelDetailShare).GetProperty(name).SetValue(model, allBlocks.Where(x => x.Key == type));
                //dynamic value = typeof(ModelDetailShare).GetProperty(name).GetValue(model);
                //await _blockModelRepository.Create(value);
            }
            

            //XDocument svg = XDocument.Parse(dto.SVG);
            //XElement element = svg.Descendants().First(x => x.Attribute("id")?.Value == StaticData.ThisSystemId.ToString());
            //element.Attribute("class").SetValue("djs-group bpmn-pool");

            bool shared = true;
            foreach (PoolDstAddressDTO pool in await _poolRepository.Addresses(id))
            {
                shared &= await CommunicationHelper.ShareModel(pool.DestinationURL, SymetricCypherHelper.JsonEncrypt(pool), "");
            }

            return "";
        }

        public Task<ModelHeaderDTO> Header(Guid id)
        {
            return _modelRepository.Header(id);
        }
    }
}
