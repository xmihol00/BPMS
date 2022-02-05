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
    public class BlockModelRepository : BaseRepository<BlockModelEntity>
    {
        public BlockModelRepository(BpmsDbContext context) : base(context) {}

        public Task<BlockModelEntity> Config(Guid id)
        {
            return _dbSet.Include(x => x.DataSchemas)
                         .Include(x => x.InFlows)
                         .FirstAsync(x => x.Id == id);
        }

    }
}