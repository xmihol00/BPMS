using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_Common.Enums;
using BPMS_DTOs.Task;

namespace BPMS_DAL.Repositories
{
    public class TaskDataRepository : BaseRepository<TaskDataEntity>
    {
        public TaskDataRepository(BpmsDbContext context) : base(context) {}

        public Task<List<TaskDataEntity>> MappedUserTasks(Guid taskId)
        {
            return _dbSet.Include(x => x.InputData)
                         .Include(x => x.Attribute)
                         .Where(x => x.AttributeId != null && x.InputData.Any(y => y.TaskId == taskId))
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> MappedServiceTasks(Guid taskId)
        {
            return _dbSet.Include(x => x.InputData)
                         .Include(x => x.Schema)
                         .Include(x => x.Attribute)
                         .Where(x => x.SchemaId != null && x.InputData.Any(y => y.TaskId == taskId))
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> Output(Guid taskId)
        {
            return _dbSet.Include(x => x.Attribute)
                         .Where(x => x.OutputTaskId == taskId)
                         .ToListAsync();
        }

        public Task<TaskDataEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}
