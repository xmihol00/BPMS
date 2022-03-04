using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_Common.Enums;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.BlockModel.ShareTypes;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Task;
using BPMS_DTOs.User;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DTOs.Account;

namespace BPMS_DAL.Repositories
{
    public class BlockModelRepository : BaseRepository<BlockModelEntity>
    {
        private readonly DbSet<ServiceTaskModelEntity> _serviceTasks;
        private readonly DbSet<UserTaskModelEntity> _userTasks;
        private readonly DbSet<RecieveEventModelEntity> _recieveEvens;
        private readonly DbSet<SendEventModelEntity> _sendEvents;

        public BlockModelRepository(BpmsDbContext context) : base(context) 
        {
            _serviceTasks = context.Set<ServiceTaskModelEntity>();
            _userTasks = context.Set<UserTaskModelEntity>();
            _recieveEvens = context.Set<RecieveEventModelEntity>();
            _sendEvents = context.Set<SendEventModelEntity>();
        }

        public Task<BlockModelEntity> Config(Guid id)
        {
            return _dbSet.Include(x => x.OutFlows)
                             .ThenInclude(x => x.InBlock)
                         .Include(x => x.InFlows)
                             .ThenInclude(x => x.OutBlock)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<string> ServiceName(Guid id)
        {
            return _serviceTasks.Include(x => x.Service)
                                .Where(x => x.Id == id)
                                .Select(x => x.Service.Name)
                                .FirstAsync();
        }

        public Task<BlockModelEntity> PreviousFlow(Guid id)
        {
            return _dbSet.Include(x => x.InFlows)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<BlockAttributeDTO>> MappedAttributes(Guid? id)
        {
            return _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Select(x => new BlockAttributeDTO 
                         {
                             Compulsory = x.Compulsory,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             Specification = x.Specification,
                             Type = x.Type,
                         })
                         .ToListAsync();
        }

        public Task<List<UserIdNameDTO>> UserIdNames<T>(Guid id, Guid agendaId, DbSet<T> set) where T : TaskModelEntity
        {
            return set.Include(x => x.Role)
                         .ThenInclude(x => x.AgendaRoles)
                             .ThenInclude(x => x.UserRoles)
                                 .ThenInclude(x => x.User)
                      .Where(x => x.Id == id)
                      .SelectMany(x => x.Role.AgendaRoles)
                      .Where(x => x.AgendaId == agendaId)
                      .SelectMany(x => x.UserRoles)
                      .Select(x => new UserIdNameDTO
                      {
                          FullName = $"{x.User.Title} {x.User.Name} {x.User.Surname}",
                          Id = x.UserId
                      })
                      .ToListAsync();
        }

        public Task<List<BlockIdNameDTO>> SenderBlocks(Guid poolId)
        {
            return _sendEvents.Where(x => x.PoolId == poolId)
                              .Select(x => new BlockIdNameDTO
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              })
                              .ToListAsync();
        }

        public Task<List<UserIdNameDTO>> UserIdNamesUser(Guid id, Guid agendaId)
        {
            return UserIdNames(id, agendaId, _userTasks);
        }

        public Task<List<UserIdNameDTO>> UserIdNamesService(Guid id, Guid agendaId)
        {
            return UserIdNames(id, agendaId, _serviceTasks);
        }

        public async Task<IEnumerable<IGrouping<Type, BlockModelEntity>>> ShareBlocks(Guid modelId)
        {
            return (await _dbSet.Include(x => x.Pool)
                                .Where(x => x.Pool.ModelId == modelId)
                                .ToListAsync())
                                .GroupBy(x => x.GetType());
        }

        public Task<List<RecieveEventShareDTO>> ShareRecieveEvents(Guid modelId)
        {
            return _context.Set<RecieveEventModelEntity>()
                           .Include(x => x.Pool)
                           .Where(x => x.Pool.ModelId == modelId)
                           .Select(x => new RecieveEventShareDTO
                           {
                               Description = x.Description,
                               Id = x.Id,
                               Name = x.Name,
                               PoolId = x.PoolId,
                               Editable = x.Editable,
                               SenderId = x.SenderId
                           })
                           .ToListAsync();
        }

        public Task<List<BlockModelEntity?>> PreviousBlock(Guid id)
        {
            return _dbSet.Include(x => x.InFlows)
                            .ThenInclude(x => x.OutBlock)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.InFlows)
                         .Select(x => x.OutBlock)
                         .ToListAsync();
        }

        public Task<TimeSpan> UserTaskDifficulty(Guid id)
        {
            return _userTasks.Where(x => x.Id == id)
                             .Select(x => x.Difficulty)
                             .FirstAsync();
        }

        public Task<UserTaskModelEntity> UserTaskForSolve(Guid id)
        {
            return _userTasks.FirstAsync(x => x.Id == id);
        }

        public Task<ServiceTaskModelEntity> ServiceTaskForSolve(Guid id)
        {
            return _serviceTasks.FirstAsync(x => x.Id == id);
        }

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> MappedInputAttributes(Guid blockId, Guid? id, string blockName, bool compulsoryAttributes)
        {
            return (await _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                                .ThenInclude(x => x.MappedBlocks)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Select(x => new InputBlockAttributeDTO 
                         {
                             Compulsory = x.Compulsory && compulsoryAttributes,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             Specification = x.Specification,
                             Type = x.Type,
                             BlockName = blockName,
                             Mapped = x.MappedBlocks.Any(x => x.BlockId == blockId)
                         })
                         .ToListAsync())
                         .GroupBy(x => x.BlockName)
                         .ToList();
        }

        public Task<BlockModelEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<BlockTypePoolIdDTO> BlockTypePoolId(Guid blockId)
        {
            return _dbSet.Where(x => x.Id == blockId)
                        .Select(x => new BlockTypePoolIdDTO 
                        {
                            PoolId = x.PoolId,
                            Type = x.GetType()
                        })
                        .FirstAsync();
        }

        public Task<List<UserTaskModelEntity>> RolesForRemovalUserTaks(Guid roleId)
        {
            return _context.Set<UserTaskModelEntity>()
                           .Where(x => x.RoleId == roleId)
                           .ToListAsync();
        }

        public Task<List<ServiceTaskModelEntity>> RolesForRemovalServiceTaks(Guid roleId)
        {
            return _context.Set<ServiceTaskModelEntity>()
                           .Where(x => x.RoleId == roleId)
                           .ToListAsync();
        }

        public Task<List<BlockModelEntity>> OfType(Type type)
        {
            return _dbSet.Where(x => x.GetType() == type)
                         .ToListAsync();
        }

        public Task<List<ServiceTaskDataSchemaDTO>> ServiceOutputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return _serviceTasks.Include(x => x.Service)
                                    .ThenInclude(x => x.DataSchemas)
                                        .ThenInclude(x => x.Blocks)
                                .Where(x => x.PoolId == poolId && x.Order < order)
                                .Select(x => new ServiceTaskDataSchemaDTO 
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Attributes = x.Service.DataSchemas
                                                  .Where(y => y.Direction == DirectionEnum.Output && y.StaticData == null && y.Type != DataTypeEnum.Object)
                                                  .Select(y => new DataSchemaAttributeDTO
                                                  {
                                                      Id = y.Id,
                                                      Compulsory = y.Compulsory,
                                                      Name = y.Name,
                                                      Type = y.Type,
                                                      Mapped = y.Blocks.Any(z => z.BlockId == blockId && z.ServiceTaskId == x.Id)
                                                  })
                                                  .ToList()
                                })
                                .ToListAsync();;
        }

        public Task<SenderRecieverConfigDTO> SenderInfo(Guid id)
        {
            return _sendEvents.Include(x => x.Pool)
                                 .ThenInclude(x => x.Model)
                              .Include(x => x.Pool)
                                 .ThenInclude(x => x.System)
                              .Where(x => x.Id == id)
                              .Select(x => new SenderRecieverConfigDTO
                              {
                                  BlockName = x.Name,
                                  Editable = false,
                                  ModelName = x.Pool.Model.Name,
                                  PoolName = x.Pool.Name,
                                  SystemName = x.Pool.System.Name
                              })
                              .FirstAsync();
        }

        public Task<List<SenderRecieverAddressDTO>> RecieverAddresses(Guid id)
        {
            return _sendEvents.Include(x => x.ForeignRecievers)
                                  .ThenInclude(x => x.System)
                              .SelectMany(x => x.ForeignRecievers)
                              .Select(x => new SenderRecieverAddressDTO
                              {
                                  DestinationURL = x.System.URL,
                                  ForeignBlockId = x.ForeignBlockId,
                                  Key = x.System.Key,
                                  SystemId = x.System.Id
                              })
                              .ToListAsync();
        }

        public Task<List<SenderRecieverConfigDTO>> RecieversInfo(Guid id)
        {
            return _sendEvents.Include(x => x.Recievers)
                                  .ThenInclude(x => x.Pool)
                                      .ThenInclude(x => x.Model)
                              .Include(x => x.Recievers)
                                  .ThenInclude(x => x.Pool)
                                      .ThenInclude(x => x.System)
                              .Where(x => x.Id == id)
                              .SelectMany(x => x.Recievers)
                              .Select(x => new SenderRecieverConfigDTO
                              {
                                  BlockName = x.Name,
                                  ModelName = x.Pool.Model.Name,
                                  PoolName = x.Pool.Name,
                                  SystemName = x.Pool.System.Name
                              })
                              .ToListAsync();
        }

        public Task<List<ServiceTaskDataSchemaDTO>> ServiceInputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return _serviceTasks.Include(x => x.Service)
                                    .ThenInclude(x => x.DataSchemas)
                                        .ThenInclude(x => x.Blocks)
                                .Where(x => x.PoolId == poolId && x.Order > order)
                                .Select(x => new ServiceTaskDataSchemaDTO 
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Attributes = x.Service.DataSchemas
                                                  .Where(y => y.Direction == DirectionEnum.Input && y.StaticData == null && y.Type != DataTypeEnum.Object)
                                                  .Select(y => new DataSchemaAttributeDTO
                                                  {
                                                      Id = y.Id,
                                                      Compulsory = y.Compulsory,
                                                      Name = y.Name,
                                                      Type = y.Type,
                                                      Mapped = y.Blocks.Any(z => z.BlockId == blockId && z.ServiceTaskId == x.Id)
                                                  })
                                                  .ToList()
                                })
                                .ToListAsync();
        }

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> TaskInputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return (await _userTasks.Include(x => x.Attributes)
                                       .ThenInclude(x => x.MappedBlocks)
                                    .Where(x => x.PoolId == poolId && x.Order < order)
                                    .SelectMany(x => x.Attributes)
                                    .Select(x => new InputBlockAttributeDTO
                                    {
                                        BlockName = x.Block.Name,
                                        Compulsory = x.Compulsory,
                                        Description = x.Description,
                                        Id = x.Id,
                                        Name = x.Name,
                                        Specification = x.Specification,
                                        Type = x.Type,
                                        Mapped = x.MappedBlocks.Any(x => x.BlockId == blockId)
                                    }).ToListAsync())
                                    .GroupBy(x => x.BlockName)
                                    .ToList();
        }

        public async Task<List<IGrouping<string, InputBlockAttributeDTO>>> RecieveEventAttribures(Guid blockId, uint order, Guid poolId)
        {
            return (await _recieveEvens.Include(x => x.Attributes)
                                          .ThenInclude(x => x.MappedBlocks)
                                       .Where(x => x.PoolId == poolId && x.Order < order)
                                       .SelectMany(x => x.Attributes)
                                       .Select(x => new InputBlockAttributeDTO
                                       {
                                           BlockName = x.Block.Name,
                                           Compulsory = x.Compulsory,
                                           Description = x.Description,
                                           Id = x.Id,
                                           Name = x.Name,
                                           Specification = x.Specification,
                                           Type = x.Type,
                                           Mapped = x.MappedBlocks.Any(x => x.BlockId == blockId)
                                       }).ToListAsync())
                                       .GroupBy(x => x.BlockName)
                                       .ToList();
        }

        public Task<bool> IsInModel(Guid blockModelId, Guid modelId)
        {
            return _dbSet.Include(x => x.Pool)
                         .AnyAsync(x => x.Id == blockModelId && x.Pool.ModelId == modelId);
        }
    }
}
