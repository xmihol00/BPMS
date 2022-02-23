using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.BlockModel.ShareTypes;
using BPMS_DTOs.ServiceDataSchema;

namespace BPMS_DAL.Repositories
{
    public class BlockModelRepository : BaseRepository<BlockModelEntity>
    {
        private readonly DbSet<ServiceTaskModelEntity> _serviceTasks;
        private readonly DbSet<UserTaskModelEntity> _userTasks;
        public BlockModelRepository(BpmsDbContext context) : base(context) 
        {
            _serviceTasks = context.Set<ServiceTaskModelEntity>();
            _userTasks = context.Set<UserTaskModelEntity>();
        }

        public Task<BlockModelEntity> Config(Guid id)
        {
            return _dbSet.Include(x => x.OutFlows)
                             .ThenInclude(x => x.InBlock)
                         .Include(x => x.InFlows)
                             .ThenInclude(x => x.OutBlock)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<BlockModelEntity> PreviousFlow(Guid id)
        {
            return _dbSet.Include(x => x.InFlows)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<BlockAttributeDTO>> MappedAttributes(Guid? id)
        {
            return _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Select(x => new BlockAttributeDTO 
                         {
                             Compulsory = x.Compulsory,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             Specification = x.Specification,
                             Type = x.Type,
                         })
                         .ToListAsync();
        }

        public async Task<IEnumerable<IGrouping<Type, BlockModelEntity>>> ShareBlocks(Guid modelId)
        {
            return (await _dbSet.Include(x => x.Pool)
                                .Where(x => x.Pool.ModelId == modelId)
                                .ToListAsync())
                                .GroupBy(x => x.GetType());
        }

        public Task<List<RecieveEventShareDTO>> ShareRecieveEvents(Guid modelId)
        {
            return _context.Set<RecieveEventModelEntity>()
                           .Include(x => x.Pool)
                           .Where(x => x.Pool.ModelId == modelId)
                           .Select(x => new RecieveEventShareDTO
                           {
                               Description = x.Description,
                               Id = x.Id,
                               Name = x.Name,
                               PoolId = x.PoolId,
                               Editable = x.Editable,
                               SenderId = x.SenderId
                           })
                           .ToListAsync();
        }

        public Task<List<BlockModelEntity?>> PreviousBlock(Guid id)
        {
            return _dbSet.Include(x => x.InFlows)
                            .ThenInclude(x => x.OutBlock)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.InFlows)
                         .Select(x => x.OutBlock)
                         .ToListAsync();
        }

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> MappedInputAttributes(Guid blockId, Guid? id, string blockName, bool compulsoryAttributes)
        {
            return (await _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                                .ThenInclude(x => x.MappedAttributes)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Select(x => new InputBlockAttributeDTO 
                         {
                             Compulsory = x.Compulsory && compulsoryAttributes,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             Specification = x.Specification,
                             Type = x.Type,
                             BlockName = blockName,
                             Mapped = x.MappedAttributes.Any(x => x.BlockId == blockId)
                         })
                         .ToListAsync())
                         .GroupBy(x => x.BlockName)
                         .ToList();
        }

        public Task<BlockModelEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<BlockTypePoolIdDTO> BlockTypePoolId(Guid blockId)
        {
            return _dbSet.Where(x => x.Id == blockId)
                        .Select(x => new BlockTypePoolIdDTO 
                        {
                            PoolId = x.PoolId,
                            Type = x.GetType()
                        })
                        .FirstAsync();
        }

        public Task<List<UserTaskModelEntity>> RolesForRemovalUserTaks(Guid roleId)
        {
            return _context.Set<UserTaskModelEntity>()
                           .Where(x => x.RoleId == roleId)
                           .ToListAsync();
        }

        public Task<List<ServiceTaskModelEntity>> RolesForRemovalServiceTaks(Guid roleId)
        {
            return _context.Set<ServiceTaskModelEntity>()
                           .Where(x => x.RoleId == roleId)
                           .ToListAsync();
        }

        public Task<List<BlockModelEntity>> OfType(Type type)
        {
            return _dbSet.Where(x => x.GetType() == type)
                         .ToListAsync();
        }

        public Task<List<DataSchemaAttributeDTO>> ServiceOutputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return _serviceTasks.Include(x => x.Service)
                                    .ThenInclude(x => x.DataSchemas)
                                        .ThenInclude(x => x.Blocks)
                                .Where(x => x.PoolId == poolId && x.Order < order)
                                .SelectMany(x => x.Service.DataSchemas)
                                .Where(x => x.Direction == DirectionEnum.Output)
                                .Select(x => new DataSchemaAttributeDTO
                                {
                                    Id = x.Id,
                                    Compulsory = x.Compulsory,
                                    Name = x.Name,
                                    Type = x.Type,
                                    Mapped = x.Blocks.Any(x => x.BlockId == blockId)
                                })
                                .ToListAsync();
        }

        public Task<List<DataSchemaAttributeDTO>> ServiceInputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return _serviceTasks.Include(x => x.Service)
                                    .ThenInclude(x => x.DataSchemas)
                                        .ThenInclude(x => x.Blocks)
                                .Where(x => x.PoolId == poolId && x.Order > order)
                                .SelectMany(x => x.Service.DataSchemas)
                                .Where(x => x.Direction == DirectionEnum.Input)
                                .Select(x => new DataSchemaAttributeDTO
                                {
                                    Id = x.Id,
                                    Compulsory = x.Compulsory,
                                    Name = x.Name,
                                    Type = x.Type,
                                    Mapped = x.Blocks.Any(x => x.BlockId == blockId)
                                })
                                .ToListAsync();
        }

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> TaskInputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return (await _userTasks.Include(x => x.Attributes)
                                       .ThenInclude(x => x.MappedAttributes)
                                    .Where(x => x.PoolId == poolId && x.Order < order)
                                    .SelectMany(x => x.Attributes)
                                    .Select(x => new InputBlockAttributeDTO
                                    {
                                        BlockName = x.Block.Name,
                                        Compulsory = x.Compulsory,
                                        Description = x.Description,
                                        Id = x.Id,
                                        Name = x.Name,
                                        Specification = x.Specification,
                                        Type = x.Type,
                                        Mapped = x.MappedAttributes.Any(x => x.BlockId == blockId)
                                    }).ToListAsync())
                                    .GroupBy(x => x.BlockName)
                                    .ToList();
        }
    }
}
