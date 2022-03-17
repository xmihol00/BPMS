using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Repositories
{
    public class AttributeMapRepository : BaseRepository<AttributeMapEntity>
    {
        public AttributeMapRepository(BpmsDbContext context) : base(context) {}

        public Task<bool> Any(Guid blockId, Guid attributeId)
        {
            return _dbSet.AnyAsync(x => x.BlockId == blockId && x.AttributeId == attributeId);
        }

        public Task<List<BlockModelEntity?>> MappedBlocks(Guid attributeId)
        {
            return _dbSet.Include(x => x.Block)
                         .Where(x => x.AttributeId == attributeId)
                         .Select(x => x.Block)
                         .ToListAsync();
        }
    }
}
