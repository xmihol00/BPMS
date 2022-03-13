using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Error;
using BPMS_DTOs.ConnectionRequest;

namespace BPMS_DAL.Repositories
{
    public class ConnectionRequestRepository : BaseRepository<ConnectionRequestEntity>
    {
        public ConnectionRequestRepository(BpmsDbContext context) : base(context) {}

        public Task<LastConnectionRequestDTO> Last(Guid systemId)
        {
            return _dbSet.Where(x => x.SystemId == systemId)
                         .Select(x => new LastConnectionRequestDTO
                         {
                             Date = x.Date,
                             Id = x.Id,
                             Text = x.Text,
                             UserName = x.SenderName
                         })
                         .OrderByDescending(x => x.Date)
                         .FirstAsync();
        }
    }
}
