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

        public Task<List<FlowEntity>> Share(Guid modelId)
        {
            return _dbSet.Include(x => x.InBlock)
                            .ThenInclude(x => x.Pool)
                         .Where(x => x.InBlock.Pool.ModelId == modelId)
                         .ToListAsync();
        }

        public Task<List<FlowEntity>> InFlows(Guid outBlockId)
        {
            return _dbSet.Include(x => x.OutBlock)
                         .Where(x => x.InBlockId == outBlockId)
                         .ToListAsync();
        }

        public Task<List<FlowEntity>> OutFlows(Guid inBlockId)
        {
            return _dbSet.Include(x => x.InBlock)
                         .Where(x => x.OutBlockId == inBlockId)
                         .ToListAsync();
        }
    }
}
