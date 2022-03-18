using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Attribute;

namespace BPMS_DAL.Repositories
{
    public class TaskDataMapRepository : BaseRepository<TaskDataMapEntity>
    {
        public TaskDataMapRepository(BpmsDbContext context) : base(context) {}

        public Task<List<TaskDataEntity?>> MappedUserTaskData(Guid id)
        {
            return _dbSet.Include(x => x.TaskData)
                            .ThenInclude(x => x.Attribute)
                         .Include(x => x.TaskData)
                            .ThenInclude(x => x.OutputTask)
                                .ThenInclude(x => x.BlockModel)
                         .Where(x => x.TaskId == id)
                         .Select(x => x.TaskData)
                         .Where(x => x.AttributeId != null)
                         .ToListAsync();
        }
    }
}
