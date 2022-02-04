using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class ModelFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly UserRepository _userRepository;

        private readonly IMapper _mapper;

        public ModelFacade(UserRepository userRepository, ModelRepository modelRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _mapper = mapper;
        }

        public Task<ModelDetailDTO> Detail(Guid id)
        {
            return _modelRepository.Detail(id);
        }

        public Task<ModelHeaderDTO> Header(Guid id)
        {
            return _modelRepository.Header(id);
        }
    }
}
