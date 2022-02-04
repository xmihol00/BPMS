using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.BlockDataSchema;

namespace BPMS_DAL.Repositories
{
    public class BlockDataSchemaRepository : BaseRepository<BlockDataSchemaEntity>
    {
        public BlockDataSchemaRepository(BpmsDbContext context) : base(context) {}

        public Task<List<BlockDataSchemaNodeDTO>> DataSchemas(Guid blockId)
        {
            return _dbSet.Where(x => x.BlockId == blockId)
                         .Select(x => new BlockDataSchemaNodeDTO 
                         {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Alias = x.Alias,
                            Name = x.Name,
                            Compulsory = x.Compulsory,
                            DataType = x.DataType,
                         })
                         .ToListAsync();
        }
    }
}
