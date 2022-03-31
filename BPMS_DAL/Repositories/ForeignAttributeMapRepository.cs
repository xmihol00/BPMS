using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Repositories
{
    public class ForeignAttributeMapRepository : BaseRepository<ForeignAttributeMapEntity>
    {
        public ForeignAttributeMapRepository(BpmsDbContext context) : base(context) {}

        public Task<List<AttributeEntity?>> ForeignAttribs(Guid foreignAttributeId)
        {
            return _dbSet.Include(x => x.Attribute)
                         .Where(x => x.ForeignAttributeId == foreignAttributeId)
                         .Select(x => x.Attribute)
                         .ToListAsync();
        }

        public Task<List<ForeignAttributeMapEntity>> ForRemoval(Guid foreignAttributeId)
        {
            return _dbSet.Include(x => x.Attribute)
                            .ThenInclude(x => x.MappedBlocks)
                         .Include(x => x.Attribute)
                            .ThenInclude(x => x.Data)
                         .Where(x => x.ForeignAttributeId == foreignAttributeId)
                         .ToListAsync();
        }

        public Task<bool> Any(Guid foreignAttributeId, Guid foreignSendEventId)
        {
            return _dbSet.AnyAsync(x => x.ForeignAttributeId == foreignAttributeId && x.ForeignSendEventId == foreignSendEventId);
        }
    }
}
