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
using BPMS_DTOs.Attribute;
using BPMS_DTOs.BlockModel;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DTOs.BlockModel.ShareTypes;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.Task;
using BPMS_DTOs.User;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DTOs.Account;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Repositories
{
    public class BlockModelRepository : BaseRepository<BlockModelEntity>
    {
        private readonly DbSet<ServiceTaskModelEntity> _serviceTasks;
        private readonly DbSet<UserTaskModelEntity> _userTasks;
        private readonly DbSet<RecieveSignalEventModelEntity> _recieveSignalEvens;
        private readonly DbSet<RecieveMessageEventModelEntity> _recieveMessageEvens;
        private readonly DbSet<SendSignalEventModelEntity> _sendSignalEvents;
        private readonly DbSet<SendMessageEventModelEntity> _sendMessageEvents;

        public BlockModelRepository(BpmsDbContext context) : base(context) 
        {
            _serviceTasks = context.Set<ServiceTaskModelEntity>();
            _userTasks = context.Set<UserTaskModelEntity>();
            _recieveSignalEvens = context.Set<RecieveSignalEventModelEntity>();
            _recieveMessageEvens = context.Set<RecieveMessageEventModelEntity>();
            _sendSignalEvents = context.Set<SendSignalEventModelEntity>();
            _sendMessageEvents = context.Set<SendMessageEventModelEntity>();
        }

        public Task<BlockModelEntity> Config(Guid id)
        {
            return _dbSet.Include(x => x.OutFlows)
                             .ThenInclude(x => x.InBlock)
                         .Include(x => x.InFlows)
                             .ThenInclude(x => x.OutBlock)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<UserIdNameDTO>> UserIdNamesRole(Guid blockId, Guid agendaId)
        {
            return _dbSet.Include(x => x.Lane)
                            .ThenInclude(x => x.Role)
                                .ThenInclude(x => x.AgendaRoles)
                                    .ThenInclude(x => x.UserRoles)
                                        .ThenInclude(x => x.User)
                         .Select(x => x.Lane.Role)
                         .SelectMany(x => x.AgendaRoles)
                         .Where(x => x.AgendaId == agendaId)
                         .SelectMany(x => x.UserRoles)
                         .Select(x => x.User)
                         .Select(x => new UserIdNameDTO
                         {
                             Id = x.Id,
                             FullName = $"{x.Title} {x.Name} {x.Surname}"
                         })
                         .ToListAsync();
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

        public Task<List<AttributeDTO>> MappedAttributes(Guid? id)
        {
            return _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Select(x => new AttributeDTO 
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

        public Task<List<BlockIdNameDTO>> SenderBlocks(Guid poolId)
        {
            return _sendSignalEvents.Include(x => x.Pool)
                                    .Where(x => x.PoolId == poolId && x.Pool.SystemId == StaticData.ThisSystemId)
                                    .Select(x => new BlockIdNameDTO
                                    {
                                        Id = x.Id,
                                        Name = x.Name
                                    })
                                    .ToListAsync();
        }

        public Task<BlockModelEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<IGrouping<Type, BlockModelEntity>>> ShareBlocks(Guid modelId)
        {
            return (await _dbSet.Include(x => x.Pool)
                                .Where(x => x.Pool.ModelId == modelId)
                                .ToListAsync())
                                .GroupBy(x => x.GetType());
        }

        public  Task<List<AttributeEntity?>> MappedBareAttributes(Guid id)
        {
            return _dbSet.Include(x => x.MappedAttributes)
                             .ThenInclude(x => x.Attribute)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Where(x => !x.Disabled)
                         .ToListAsync();
        }

        public Task<List<RecieveEventShareDTO>> ShareRecieveEvents(Guid modelId)
        {
            return _context.Set<RecieveMessageEventModelEntity>()
                           .Include(x => x.Pool)
                           .Where(x => x.Pool.ModelId == modelId)
                           .Select(x => new RecieveEventShareDTO
                           {
                               Description = x.Description,
                               Id = x.Id,
                               Name = x.Name,
                               PoolId = x.PoolId,
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

        public Task<int> UserTaskDifficulty(Guid id)
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

        public async Task<List<IGrouping<string, InputAttributeDTO>>> MappedInputAttributes(Guid blockId, Guid? id, string blockName, bool compulsoryAttributes)
        {
            return (await _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                                .ThenInclude(x => x.MappedBlocks)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.MappedAttributes)
                         .Select(x => x.Attribute)
                         .Select(x => new InputAttributeDTO 
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

        public Task<List<BlockModelDataSchemaEntity>> DataShemas(Guid id)
        {
            return _dbSet.Include(x => x.DataSchemas)
                            .ThenInclude(x => x.DataSchema)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.DataSchemas)
                         .Where(x => !x.DataSchema.Disabled)
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
                                                  .Where(y => y.Direction == DirectionEnum.Output && y.StaticData == null && y.Type != DataTypeEnum.Object &&
                                                              y.Array == false && !y.Disabled)
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

        public Task<SenderRecieverConfigDTO> SenderMessageInfo(Guid id)
        {
            return _sendMessageEvents.Include(x => x.Pool)
                                        .ThenInclude(x => x.Model)
                                     .Include(x => x.Pool)
                                        .ThenInclude(x => x.System)
                                     .Where(x => x.RecieverId == id)
                                     .Select(x => new SenderRecieverConfigDTO
                                     {
                                         SystemName = x.Pool.System.Name,
                                         BlockName = x.Name,
                                         ModelName = x.Pool.Model.Name,
                                         PoolName = x.Pool.Name
                                     })
                                     .FirstAsync();
        }

        public Task<SenderRecieverConfigDTO> SenderSignalInfo(Guid id)
        {
            return _sendSignalEvents.Include(x => x.Pool)
                                       .ThenInclude(x => x.Model)
                                    .Include(x => x.Pool)
                                       .ThenInclude(x => x.System)
                                    .Where(x => x.Id == id)
                                    .Select(x => new SenderRecieverConfigDTO
                                    {
                                        SystemName = x.Pool.System.Name,
                                        BlockName = x.Name,
                                        ModelName = x.Pool.Model.Name,
                                        PoolName = x.Pool.Name
                                    })
                                    .FirstAsync();
        }

        public Task<List<SenderRecieverAddressDTO>> ForeignRecieverAddresses(Guid id)
        {
            return _sendSignalEvents.Include(x => x.ForeignRecievers)
                                  .ThenInclude(x => x.System)
                              .Where(x => x.Id == id)
                              .SelectMany(x => x.ForeignRecievers)
                              .Select(x => new SenderRecieverAddressDTO
                              {
                                  DestinationURL = x.System.URL,
                                  ForeignBlockId = x.ForeignBlockId,
                                  Key = x.System.Key,
                                  SystemId = x.System.Id,
                                  SystemName = x.System.Name,
                                  Encryption = x.System.Encryption > x.System.ForeignEncryption ? x.System.Encryption : x.System.ForeignEncryption
                              })
                              .ToListAsync();
        }

        public Task<SenderRecieverConfigDTO> RecieversInfo(Guid id)
        {
            return _sendMessageEvents.Include(x => x.Reciever)
                                  .ThenInclude(x => x.Pool)
                                      .ThenInclude(x => x.Model)
                              .Include(x => x.Reciever)
                                  .ThenInclude(x => x.Pool)
                                      .ThenInclude(x => x.System)
                              .Where(x => x.Id == id)
                              .Select(x => x.Reciever)
                              .Select(x => new SenderRecieverConfigDTO
                              {
                                  SystemName = x.Pool.System.Name,
                                  BlockName = x.Name,
                                  ModelName = x.Pool.Model.Name,
                                  PoolName = x.Pool.Name,
                              })
                              .FirstAsync();
        }

        public Task<SenderRecieverConfigDTO> ForeignRecieverInfo(Guid id)
        {
            return _dbSet.Include(x => x.Pool)
                            .ThenInclude(x => x.Model)
                         .Where(x => x.Id == id)
                         .Select(x => new SenderRecieverConfigDTO
                         {
                             BlockName = x.Name,
                             ModelName = x.Pool.Model.Name,
                             PoolName = x.Pool.Name,
                         })
                         .FirstAsync();
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
                                                  .Where(y => y.Direction == DirectionEnum.Input && y.StaticData == null && y.Type != DataTypeEnum.Object &&
                                                              y.Array == false && !y.Disabled)
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

        public async Task<List<IGrouping<string, InputAttributeDTO>>> TaskInputAttributes(Guid blockId, uint order, Guid poolId)
        {
            return (await _userTasks.Include(x => x.Attributes)
                                       .ThenInclude(x => x.MappedBlocks)
                                    .Where(x => x.PoolId == poolId && x.Order < order)
                                    .SelectMany(x => x.Attributes)
                                    .Where(x => !x.Disabled)
                                    .Select(x => new InputAttributeDTO
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

        public async Task<List<IGrouping<string, InputAttributeDTO>>> RecieveSignalEventAttribures(Guid blockId, uint order, Guid poolId)
        {
            return (await _recieveSignalEvens.Include(x => x.Attributes)
                                          .ThenInclude(x => x.MappedBlocks)
                                       .Where(x => x.PoolId == poolId && x.Order < order)
                                       .SelectMany(x => x.Attributes)
                                       .Where(x => !x.Disabled)
                                       .Select(x => new InputAttributeDTO
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

        public async Task<List<IGrouping<string, InputAttributeDTO>>> RecieveMessageEventAttribures(Guid blockId, uint order, Guid poolId)
        {
            return (await _recieveMessageEvens.Include(x => x.Attributes)
                                          .ThenInclude(x => x.MappedBlocks)
                                       .Where(x => x.PoolId == poolId && x.Order < order)
                                       .SelectMany(x => x.Attributes)
                                       .Where(x => !x.Disabled)
                                       .Select(x => new InputAttributeDTO
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

        public Task<List<SenderRecieverAddressDTO>> ForeignRecieversAddresses(Guid id)
        {
            return _sendSignalEvents.Include(x => x.ForeignRecievers)
                                 .ThenInclude(x => x.System)
                              .Where(x => x.Id == id)
                              .SelectMany(x => x.ForeignRecievers)
                              .Select(x => new SenderRecieverAddressDTO
                              {
                                  DestinationURL = x.System.URL,
                                  ForeignBlockId = x.ForeignBlockId,
                                  Key = x.System.Key,
                                  SystemId = x.SystemId,
                                  Encryption = x.System.Encryption > x.System.ForeignEncryption ? x.System.Encryption : x.System.ForeignEncryption
                              })
                              .ToListAsync();
        }

        public Task<bool> IsInModel(Guid blockModelId, Guid modelId)
        {
            return _dbSet.Include(x => x.Pool)
                         .AnyAsync(x => x.Id == blockModelId && x.Pool.ModelId == modelId);
        }

        public Task<BlockAddressDTO?> RecieverAddress(Guid id)
        {
            return _sendMessageEvents.Include(x => x.Reciever)
                                .ThenInclude(x => x.Pool)
                                    .ThenInclude(x => x.System)
                              .Where(x => x.Id == id)
                              .Select(x => x.Reciever)
                              .Select(x => new BlockAddressDTO
                              {
                                  PoolId = x.PoolId,
                                  DestinationURL = x.Pool.System.URL,
                                  SystemId = x.Pool.System.Id,
                                  Key = x.Pool.System.Key,
                                  BlockId = x.Id,
                                  ModelId = x.Pool.ModelId,
                                  Encryption = x.Pool.System.Encryption > x.Pool.System.ForeignEncryption ? 
                                               x.Pool.System.Encryption : x.Pool.System.ForeignEncryption
                              })
                              .FirstOrDefaultAsync();
        }

        public Task<List<DataSchemaSourceDTO>> Sources(uint order, Guid poolId)
        {
            return _serviceTasks.Include(x => x.Service)
                                    .ThenInclude(x => x.DataSchemas)
                                .Where(x => x.PoolId == poolId && x.Order < order)
                                .Select(x => new DataSchemaSourceDTO
                                {
                                    BlockName = x.Name,
                                    Sources = x.Service.DataSchemas
                                                       .Where(x => x.StaticData == null && x.Direction == DirectionEnum.Output)
                                                       .Select(x => new DataSchemaMapDTO
                                                       {
                                                           Alias = x.Alias,
                                                           Id = x.Id,
                                                           Name = x.Name,
                                                           Type = x.Type
                                                       })
                                                       .ToList()
                                })
                                .ToListAsync();
        }

        public Task<Guid> LeastBussyUser(Guid id, Guid agendaId)
        {
            return _dbSet.Include(x => x.Lane)
                            .ThenInclude(x => x.Role)
                                .ThenInclude(x => x.AgendaRoles)
                                    .ThenInclude(x => x.UserRoles)
                                        .ThenInclude(x => x.User)
                                            .ThenInclude(x => x.Tasks)
                         .Where(x => x.Id == id)
                         .Select(x => x.Lane.Role)
                         .SelectMany(x => x.AgendaRoles)
                         .Where(x => x.AgendaId == agendaId)
                         .SelectMany(x => x.UserRoles)
                         .Select(x => x.User)
                         .OrderBy(x => x.Tasks.Count)
                         .Select(x => x.Id)
                         .FirstOrDefaultAsync();
        }
    }
}
