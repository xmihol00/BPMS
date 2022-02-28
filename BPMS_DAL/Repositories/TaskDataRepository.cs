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
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces.BlockDataTypes;
using BPMS_DAL.Entities.BlockDataTypes;

namespace BPMS_DAL.Repositories
{
    public class TaskDataRepository : BaseRepository<TaskDataEntity>
    {
        public TaskDataRepository(BpmsDbContext context) : base(context) {}

        public Task<List<TaskDataEntity>> MappedUserTaskData(Guid taskId)
        {
            return _dbSet.Include(x => x.InputData)
                         .Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.BlockModel)
                         .Where(x => x.AttributeId != null && x.InputData.Any(y => y.TaskId == taskId))
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> MappedSendEventData(Guid taskId)
        {
            return _dbSet.Include(x => x.InputData)
                         .Where(x => x.AttributeId != null && x.InputData.Any(y => y.TaskId == taskId))
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> MappedServiceTasks(Guid taskId)
        {
            return _dbSet.Include(x => x.InputData)
                         .Include(x => x.Schema)
                         .Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.BlockModel)
                         .Where(x => x.SchemaId != null && x.InputData.Any(y => y.TaskId == taskId))
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> OutputUserTasks(Guid taskId)
        {
            return _dbSet.Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.BlockModel)
                         .Where(x => x.OutputTaskId == taskId)
                         .ToListAsync();
        }

        public Task<Dictionary<Guid, TaskDataEntity>> InputServiceTaskData(Guid taskId)
        {
            return _dbSet.Include(x => x.Schema)
                         .Where(x => x.OutputTaskId == taskId && x.Schema.Direction == DirectionEnum.Input)
                         .ToDictionaryAsync(x => x.SchemaId.Value);
        }

        public Task<TaskDataEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<TaskDataEntity>> OutputServiceTaskData(Guid taskId)
        {
            return _dbSet.Include(x => x.Schema)
                         .Where(x => x.OutputTaskId == taskId && x.Schema.Direction == DirectionEnum.Output)
                         .ToListAsync();
        }

        public Task<Dictionary<Guid, TaskDataEntity>> OfRecieveEvent(Guid workflowId, Guid blockId)
        {
            return _dbSet.Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.Workflow)
                         .Where(x => x.OutputTask.WorkflowId == workflowId && x.Attribute.BlockId == blockId &&
                                     x.OutputTask.Workflow.State == WorkflowStateEnum.Active)
                         .ToDictionaryAsync(x => (Guid)x.AttributeId);
        }

        public Task<Dictionary<Guid, TaskDataEntity>> OfRecieveEvent(Guid blockId)
        {
            return _dbSet.Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.Workflow)
                         .Where(x => x.Attribute.BlockId == blockId && x.OutputTask.Workflow.State == WorkflowStateEnum.Active)
                         .ToDictionaryAsync(x => (Guid)x.AttributeId);
        }

        public Task<FileDownloadDTO> FileForDownload(Guid id)
        {
            return _context.Set<FileDataEntity>()
                           .Where(x => x.Id == id)
                           .Select(x => new FileDownloadDTO
                           {
                               FileName = x.FileName,
                               MIMEType = x.MIMEType
                           })
                           .FirstAsync();
        }
    }
}
