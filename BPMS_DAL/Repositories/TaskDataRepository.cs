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
using BPMS_DTOs.DataSchema;
using BPMS_DAL.Sharing;

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

        public async Task<Dictionary<Guid, IGrouping<Guid, TaskDataEntity>>> InputServiceTaskData(Guid taskId)
        {
            return (await _dbSet.Include(x => x.Schema)
                               .Where(x => x.OutputTaskId == taskId && x.Schema.Direction == DirectionEnum.Input)
                               .ToListAsync())
                               .GroupBy(x => x.Schema.Id)
                               .ToDictionary(x => x.Key);
        }

        public Task<TaskDataEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<TaskDataEntity>> OutputServiceTaskData(Guid taskId)
        {
            return _dbSet.Include(x => x.Schema)
                         .Where(x => x.OutputTaskId == taskId && x.Schema.Direction == DirectionEnum.Output && !x.Schema.Disabled)
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

        public Task<TaskDataEntity> BareSchema(Guid id)
        {
            return _dbSet.Include(x => x.Schema)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<Dictionary<Guid, TaskDataEntity>> OfRecieveEvent(Guid blockId)
        {
            return _dbSet.Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.Workflow)
                         .Where(x => x.Attribute.BlockId == blockId && x.OutputTask.Workflow.State == WorkflowStateEnum.Active)
                         .ToDictionaryAsync(x => x.AttributeId.Value);
        }

        public async Task<IEnumerable<IGrouping<Guid, TaskDataEntity>>> OfForeignRecieveEvent(Guid blockId)
        {
            return (await _dbSet.Include(x => x.Attribute)
                         .Include(x => x.OutputTask)
                            .ThenInclude(x => x.Workflow)
                         .Include(x => x.Attribute)
                            .ThenInclude(x => x.MappedForeignBlock)
                         .Where(x => x.Attribute.BlockId == blockId && x.OutputTask.Workflow.State == WorkflowStateEnum.Active)
                         .ToListAsync())
                         .GroupBy(x => x.Attribute.MappedForeignBlock.ForeignAttributeId);
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

        public Task<List<TaskDataEntity>> OfArray(Guid id, Guid? dataSchemaId)
        {
            return _dbSet.Where(x => x.Id != id && x.SchemaId == dataSchemaId)
                         .ToListAsync();
        }

        public Task<List<DataSchemaDataMap>> MappedServiceTaskData(Guid serviceTaskId, Guid workflowId)
        {
            return _dbSet.Include(x => x.Schema)
                            .ThenInclude(x => x.Sources)
                                .ThenInclude(x => x.Target)
                                    .ThenInclude(x => x.Data)
                                        .Include(x => x.OutputTask)
                         .Where(x => x.OutputTaskId == serviceTaskId && x.Schema.Direction == DirectionEnum.Output && !x.Schema.Disabled)
                         .Select(x => x.Schema)
                         .Select(x => new DataSchemaDataMap
                         {
                             ParentId = x.ParentId,
                             Alias = x.Alias,
                             Name = x.Name,
                             Data = x.Sources.SelectMany(x => x.Target.Data)
                                             .Where(x => x.OutputTaskId != serviceTaskId && x.OutputTask.WorkflowId == workflowId)
                                             .ToList()
                         })
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> ServiceTaskData(Guid taskId)
        {
            return _dbSet.Include(x => x.Schema)
                         .Where(x => x.OutputTaskId == taskId && x.Schema.StaticData == null)
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> ServiceTaskDataOutput(Guid taskId)
        {
            return _dbSet.Include(x => x.Schema)
                         .Where(x => x.OutputTaskId == taskId && x.Schema.Direction == DirectionEnum.Output && x.Schema.StaticData == null)
                         .ToListAsync();
        }
    }
}
