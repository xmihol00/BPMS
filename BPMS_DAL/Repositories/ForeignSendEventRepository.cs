using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Pool;
using BPMS_DTOs.Account;
using BPMS_Common;

namespace BPMS_DAL.Repositories
{
    public class ForeignSendEventRepository : BaseRepository<ForeignSendSignalEventEntity>
    {
        public ForeignSendEventRepository(BpmsDbContext context) : base(context) {}

        public Task<SenderRecieverAddressDTO> SenderAddress(Guid id)
        {
            return _dbSet.Include(x => x.System)
                         .Where(x => x.Id == id)
                         .Select(x => new SenderRecieverAddressDTO
                         {
                             DestinationURL = x.System.URL,
                             Key = x.System.Key,
                             SystemId = x.SystemId,
                             ForeignBlockId = x.ForeignBlockId,
                             SystemName = x.System.Name,
                             Encryption = x.System.Encryption > x.System.ForeignEncryption ? x.System.Encryption : x.System.ForeignEncryption
                         })
                         .FirstAsync();
        }

        public Task<ForeignSendSignalEventEntity?> Bare(Guid systemId, Guid blockId)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.SystemId == systemId && x.ForeignBlockId == blockId);
        }

        public Task<ForeignSendSignalEventEntity> ForRemoval(Guid id)
        {
            return _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                                .ThenInclude(x => x.MappedBlocks)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<ForeignSendSignalEventEntity>> BareReciever(Guid recieverId)
        {
            return _dbSet.Include(x => x.Reciever)
                         .Where(x => x.Reciever.Id == recieverId)
                         .ToListAsync();
        }
    }
}
