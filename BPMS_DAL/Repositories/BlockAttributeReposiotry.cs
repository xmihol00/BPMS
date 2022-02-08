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
    public class BlockAttributeRepository : BaseRepository<BlockAttributeEntity>
    {
        public BlockAttributeRepository(BpmsDbContext context) : base(context) {}

        public Task<List<BlockAttributeDTO>> All(Guid blockId)
        {
            return _dbSet.Where(x => x.BlockId == blockId)
                         .Select(x => new BlockAttributeDTO
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

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> InputAttributes(Guid inputId, Guid outputId)
        {
            #pragma warning disable CS8602
            return (await _dbSet.Include(x => x.MappedBlocks)
                         .Include(x => x.Block)
                         .Where(x => x.BlockId == outputId)
                         .Select(x => new InputBlockAttributeDTO
                         {
                             Compulsory = x.Compulsory,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             BlockName = x.Block.Name,
                             Specification = x.Specification,
                             Type = x.Type,
                             Mapped = x.MappedBlocks.Any(y => y.BlockId == inputId)
                         })
                         .ToListAsync())
                         .GroupBy(x => x.BlockName)
                         .ToList();
            #pragma warning restore CS8602
        }
    }
}
