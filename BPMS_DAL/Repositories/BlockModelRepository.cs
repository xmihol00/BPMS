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

namespace BPMS_DAL.Repositories
{
    public class BlockModelRepository : BaseRepository<BlockModelEntity>
    {
        public BlockModelRepository(BpmsDbContext context) : base(context) {}

        public Task<BlockModelEntity> Config(Guid id)
        {
            return _dbSet.Include(x => x.OutFlows)
                             .ThenInclude(x => x.InBlock)
                         .Include(x => x.InFlows)
                             .ThenInclude(x => x.OutBlock)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<BlockModelEntity> PreviousBlock(Guid id)
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

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> MappedInputAttributes(Guid blockId, Guid? id, string blockName, bool compulsoryAttributes)
        {
            return (await _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                                .ThenInclude(x => x.MappedBlocks)
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
                             Mapped = x.MappedBlocks.Any(x => x.BlockId == blockId)
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
    }
}
