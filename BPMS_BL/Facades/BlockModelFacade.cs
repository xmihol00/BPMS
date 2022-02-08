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
using BPMS_DAL.Interfaces;
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
        private readonly BlockAttributeMapRepository _blockAttributeMapRepository;
        private readonly IMapper _mapper;

        public BlockModelFacade(BlockModelRepository blockModelRepository, BlockAttributeRepository blockAttributeRepository,
                                BlockAttributeMapRepository blockAttributeMapRepository, IMapper mapper)
        {
            _blockModelRepository = blockModelRepository;
            _blockAttributeRepository = blockAttributeRepository;
            _blockAttributeMapRepository = blockAttributeMapRepository;
            _mapper = mapper;
        }

        public async Task Remove(Guid id)
        {
            _blockAttributeRepository.Remove(new BlockAttributeEntity { Id = id });
            await _blockAttributeRepository.Save();
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
            
            return await Config(dto.BlockId);
        }

        public async Task ToggleMap(Guid blockId, Guid attributeId)
        {
            BlockAttributeMapEntity entity = new BlockAttributeMapEntity()
            {
                AttributeId = attributeId,
                BlockId = blockId
            };

            if (await _blockAttributeMapRepository.Any(blockId, attributeId))
            {
                _blockAttributeMapRepository.Remove(entity);
            }
            else
            {   
                await _blockAttributeMapRepository.Create(entity);
            }

            await _blockAttributeMapRepository.Save();
        }

        public async Task<BlockModelConfigDTO> Config(Guid id)
        {
            BlockModelEntity entity = await _blockModelRepository.Config(id);

            BlockModelConfigDTO dto;
            #pragma warning disable CS8602, CS8600
            switch (entity)
            {
                case ITaskModelEntity:
                    dto = new UserTaskConfigDTO();
                    (dto as IAttributesConfigDTO).Attributes = await _blockAttributeRepository.All(entity.Id);                   
                    (dto as IInputAttributesConfigDTO).InputAttributes = await InputAttributes(entity.InFlows);
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
                    dto = new BlockModelConfigDTO();
                    break;

                case IParallelGatewayModelEntity parallelGateway:
                    dto = new BlockModelConfigDTO();
                    break;
                
                case ISendEventModelEntity sendEvent:
                    dto = new SendEventConfigDTO();
                    (dto as IInputAttributesConfigDTO).InputAttributes = await InputAttributes(entity.InFlows);
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

        private async Task<List<IGrouping<string, InputBlockAttributeDTO>>> InputAttributes(List<FlowEntity> flows)
        {
            List<IGrouping<string, InputBlockAttributeDTO>> inputAttributes = new List<IGrouping<string, InputBlockAttributeDTO>>();
            foreach (FlowEntity flow in flows)
            {
                FlowEntity? correctFlow = await FirstBlockWithAttributes(flow);
                if (correctFlow is not null)
                {
                    inputAttributes.AddRange(await _blockAttributeRepository.InputAttributes(correctFlow.InBlockId, correctFlow.OutBlockId));
                }
            }

            return inputAttributes;
        }

        private async Task<FlowEntity?> FirstBlockWithAttributes(FlowEntity? flow)
        {
            if (flow == null)
            {
                return null;
            }

            BlockModelEntity block = await _blockModelRepository.PreviousBlock(flow.OutBlockId);
            if (block is IAttributes)
            {
                return flow;
            }
            else if (block is INoAttributes)
            {
                return null;
            }
            else
            {
                return await FirstBlockWithAttributes(block.InFlows.FirstOrDefault());
            }
        }
    }
}
