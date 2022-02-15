using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Flow;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Repositories
{
    public class FlowRepository : BaseRepository<FlowEntity>
    {
        public FlowRepository(BpmsDbContext context) : base(context) {}

        public Task<List<FlowShareDTO>> Share(Guid modelId)
        {
            return _dbSet.Include(x => x.InBlock)
                            .ThenInclude(x => x.Pool)
                         .Where(x => x.InBlock.Pool.ModelId == modelId)
                         .Select(x => new FlowShareDTO
                         {
                             InBlockId = x.InBlockId,
                             OutBlockId = x.OutBlockId
                         })
                         .ToListAsync();
        }
    }
}
