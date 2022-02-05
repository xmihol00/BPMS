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
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockDataSchema;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class BlockModelFacade
    {
        private readonly BlockModelRepository _blockModelRepository;
        private readonly BlockDataSchemaRepository _blockDataSchemaRepository;
        private readonly IMapper _mapper;

        public BlockModelFacade(BlockModelRepository blockModelRepository, BlockDataSchemaRepository blockDataSchemaRepository,
                                IMapper mapper)
        {
            _blockModelRepository = blockModelRepository;
            _blockDataSchemaRepository = blockDataSchemaRepository;
            _mapper = mapper;
        }

        public async Task<BlockModelConfigDTO> CreateEditSchema(BlockDataSchemaCreateEditDTO dto)
        {
            BlockDataSchemaEntity entity = _mapper.Map<BlockDataSchemaEntity>(dto);
            
            if (dto.Id == Guid.Empty)
            {
                await _blockDataSchemaRepository.Create(entity);
            }
            else
            {
                _blockDataSchemaRepository.Update(entity);
            }

            await _blockDataSchemaRepository.Save();

            return await Config(dto.BlockId);
        }

        public async Task<BlockModelConfigDTO> Config(Guid id)
        {
            BlockModelConfigDTO dto = new BlockModelConfigDTO();
            BlockModelEntity entity = await _blockModelRepository.Config(id);

            dto.RootNodes = CreateTree(await _blockDataSchemaRepository.DataSchemas(id), null);

            switch (entity)
            {
                case IUserTaskModelEntity userTask:
                    break;
                
                case IServiceTaskModelEntity serviceTask:
                    break;

                case IStartEventModelEntity startEvent:
                    break;

                case IEndEventModelEntity endEvent:
                    break;
                
                case IExclusiveGatewayModelEntity exclusiveGateway:
                    break;

                case IParallelGatewayModelEntity parallelGateway:
                    break;
                
                case ISendEventModelEntity sendEvent:
                    break;
                
                case IRecieveEventModelEntity recieveEvent:
                    break;
            }

            dto.Id = entity.Id;
            dto.Name = entity.Name;

            return dto;
        }

        private IEnumerable<BlockDataSchemaNodeDTO> CreateTree(IEnumerable<BlockDataSchemaNodeDTO> allNodes, Guid? parentId)
        {
            IEnumerable<BlockDataSchemaNodeDTO> nodes = allNodes.Where(x => x.ParentId == parentId);
            foreach (BlockDataSchemaNodeDTO node in nodes)
            {
                if (node.DataType == DataTypeEnum.Object)
                {
                    node.Children = CreateTree(allNodes, node.Id);
                }
            }

            return nodes;
        }
    }
}
