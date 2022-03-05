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
    public class ForeignSendEventRepository : BaseRepository<ForeignSendEventEntity>
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
                             URL = StaticData.ThisSystemURL,
                             ForeignBlockId = x.ForeignBlockId
                         })
                         .FirstAsync();
        }

        public Task<List<Guid>> RecieverIds(Guid foreignBlockId, Guid systemId)
        {
            return _dbSet.Include(x => x.Reciever)
                         .Where(x => x.ForeignBlockId == foreignBlockId && x.SystemId == systemId)
                         .Select(x => x.Reciever)
                         .Select(x => x.Id)
                         .ToListAsync();
        }

        public Task<ForeignSendEventEntity?> Bare(Guid systemId, Guid blockId)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.SystemId == systemId && x.ForeignBlockId == blockId);
        }

        public Task<ForeignSendEventEntity> ForRemoval(Guid id)
        {
            return _dbSet.Include(x => x.MappedAttributes)
                            .ThenInclude(x => x.Attribute)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<ForeignSendEventEntity>> BareReciever(Guid foreignBlockId)
        {
            return _dbSet.Include(x => x.Reciever)
                         .Where(x => x.ForeignBlockId == foreignBlockId)
                         .ToListAsync();
        }
    }
}
