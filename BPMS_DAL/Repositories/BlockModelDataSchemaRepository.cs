using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.BlockAttribute;

namespace BPMS_DAL.Repositories
{
    public class BlockModelDataSchemaRepository : BaseRepository<BlockModelDataSchemaEntity>
    {
        public BlockModelDataSchemaRepository(BpmsDbContext context) : base(context) {}
       
        public Task<bool> Any(Guid blockId, Guid dataSchemaId, Guid serviceTaskId)
        {
            return _dbSet.AnyAsync(x => x.BlockId == blockId && x.DataSchemaId == dataSchemaId && x.ServiceTaskId == serviceTaskId);
        }
    }
}
