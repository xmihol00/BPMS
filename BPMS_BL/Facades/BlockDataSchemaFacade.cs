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
using BPMS_DTOs.BlockDataSchema;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class BlockDataSchemaFacade
    {
        private readonly BlockDataSchemaRepository _dataSchemaRepository;
        private readonly IMapper _mapper;

        public BlockDataSchemaFacade(BlockDataSchemaRepository dataSchemaRepository, IMapper mapper)
        {
            _dataSchemaRepository = dataSchemaRepository;
            _mapper = mapper;
        }
    }
}
