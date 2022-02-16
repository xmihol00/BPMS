using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.User;
using Newtonsoft.Json;

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
            ModelEntity dto = await _modelRepository.Share(id);
            //dto.Pools = await _poolRepository.Share(id);
            //dto.Flows = await _flowRepository.Share(id);
            //dto.Blocks = await _blockModelRepository.ShareBlocks(id);
            //dto.Blocks.AddRange(await _blockModelRepository.ShareRecieveEvents(id));

            string model = JsonConvert.SerializeObject(dto);
            //XDocument svg = XDocument.Parse(dto.SVG);
            //XElement element = svg.Descendants().First(x => x.Attribute("id")?.Value == StaticData.ThisSystemId.ToString());
            //element.Attribute("class").SetValue("djs-group bpmn-pool");

            bool shared = true;
            foreach (PoolDstAddressDTO pool in await _poolRepository.Addresses(id))
            {
                shared &= await CommunicationHelper.ShareModel(pool.DestinationURL, SymetricCypherHelper.JsonEncrypt(pool), model);
            }

            return model;
        }

        public Task<ModelHeaderDTO> Header(Guid id)
        {
            return _modelRepository.Header(id);
        }
    }
}
