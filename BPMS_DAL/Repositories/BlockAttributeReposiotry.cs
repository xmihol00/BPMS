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
    public class BlockAttributeRepository : BaseRepository<TaskAttributeEntity>
    {
        public BlockAttributeRepository(BpmsDbContext context) : base(context) {}

        public Task<List<BlockAttributeDTO>> All(Guid taskId)
        {
            return _dbSet.Where(x => x.TaskId == taskId)
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
    }
}
