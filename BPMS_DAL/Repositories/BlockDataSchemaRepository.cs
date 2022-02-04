using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.DataSchema;

namespace BPMS_DAL.Repositories
{
    public class BlockDataSchemaRepository : BaseRepository<BlockDataSchemaEntity>
    {
        public BlockDataSchemaRepository(BpmsDbContext context) : base(context) {}

        public Task<List<DataSchemaNodeDTO>> DataSchemas(Guid blockId)
        {
            return _dbSet.Where(x => x.BlockId == blockId)
                         .Select(x => new DataSchemaNodeDTO 
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
