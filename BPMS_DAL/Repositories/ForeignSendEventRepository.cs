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
    }
}
