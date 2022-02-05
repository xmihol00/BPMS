using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockModel.ConfigTypes;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class BlockModelFacade
    {
        private readonly BlockModelRepository _blockModelRepository;
        private readonly BlockAttributeRepository _blockAttributeRepository;
        private readonly IMapper _mapper;

        public BlockModelFacade(BlockModelRepository blockModelRepository, BlockAttributeRepository blockAttributeRepository,
                                IMapper mapper)
        {
            _blockModelRepository = blockModelRepository;
            _blockAttributeRepository = blockAttributeRepository;
            _mapper = mapper;
        }

        public async Task<BlockModelConfigDTO> CreateEdit(AttributeCreateEditDTO dto)
        {
            BlockAttributeEntity entity = _mapper.Map<BlockAttributeEntity>(dto);
            if (dto.Id == Guid.Empty)
            {
                await _blockAttributeRepository.Create(entity);
            }
            else
            {
                _blockAttributeRepository.Update(entity);
            }

            await _blockAttributeRepository.Save();
            return await Config(dto.TaskId);
        }

        public async Task<BlockModelConfigDTO> Config(Guid id)
        {
            BlockModelEntity entity = await _blockModelRepository.Config(id);

            BlockModelConfigDTO dto;
            #pragma warning disable CS8602, CS8600
            switch (entity)
            {
                case ITaskModelEntity userTask:
                    dto = new UserTaskConfigDTO();
                    (dto as IUserTaskConfigDTO).Attributes = await _blockAttributeRepository.All(entity.Id);
                    break;
                
                case IServiceTaskModelEntity serviceTask:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;

                case IStartEventModelEntity startEvent:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;

                case IEndEventModelEntity endEvent:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;
                
                case IExclusiveGatewayModelEntity exclusiveGateway:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;

                case IParallelGatewayModelEntity parallelGateway:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;
                
                case ISendEventModelEntity sendEvent:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;
                
                case IRecieveEventModelEntity recieveEvent:
                    dto = new BlockModelConfigDTO(); //TODO
                    break;
                
                default:
                    dto = new BlockModelConfigDTO();
                    break;
            }
            #pragma warning restore CS8602, CS8600

            dto.Id = entity.Id;
            dto.Name = entity.Name;

            return dto;
        }
    }
}
