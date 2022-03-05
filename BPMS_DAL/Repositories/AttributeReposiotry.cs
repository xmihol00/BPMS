using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Attribute;

namespace BPMS_DAL.Repositories
{
    public class AttributeRepository : BaseRepository<AttributeEntity>
    {
        public AttributeRepository(BpmsDbContext context) : base(context) {}

        public Task<List<AttributeDTO>> Details(Guid blockId)
        {
            return _dbSet.Where(x => x.BlockId == blockId)
                         .Select(x => new AttributeDTO
                         {
                             Compulsory = x.Compulsory,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             Specification = x.Specification,
                             Type = x.Type
                         })
                         .ToListAsync();
        }


        public Task<List<AttributeAllDTO>> All(Guid id)
        {
            return _dbSet.Where(x => x.BlockId == id)
                         .Select(x => new AttributeAllDTO
                         {
                             Id = x.Id,
                             Type = x.Type
                         })
                         .ToListAsync();
        }

        public async Task<List<IGrouping<string, InputAttributeDTO>>> InputAttributes(Guid blockId, Guid outputId, bool compulsoryAttributes)
        {
            return (await _dbSet.Include(x => x.MappedBlocks)
                         .Include(x => x.Block)
                         .Where(x => x.BlockId == outputId)
                         .Select(x => new InputAttributeDTO
                         {
                             Compulsory = x.Compulsory && compulsoryAttributes,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             BlockName = x.Block.Name,
                             Specification = x.Specification,
                             Type = x.Type,
                             Mapped = x.MappedBlocks.Any(y => y.BlockId == blockId)
                         })
                         .ToListAsync())
                         .GroupBy(x => x.BlockName)
                         .ToList();
        }

        public Task<List<AttributeMapEntity>> MapsFromDifferentPool(Guid attribId, Guid poolId)
        {
            return _dbSet.Where(x => x.Id == attribId)
                         .Include(x => x.MappedBlocks)
                            .ThenInclude(x => x.Block)
                         .SelectMany(x => x.MappedBlocks)
                         .Where(x => x.Block.PoolId != poolId)
                         .ToListAsync();
        }

        public Task<bool> Any(Guid id)
        {
            return _dbSet.AnyAsync(x => x.Id == id);
        }

        public Task<AttributeEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<AttributeMapEntity>> MappedBlocks(Guid id)
        {
            return _dbSet.Include(x => x.MappedBlocks)
                            .ThenInclude(x => x.Block)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedBlocks)
                         .ToListAsync();
        }
    }
}
