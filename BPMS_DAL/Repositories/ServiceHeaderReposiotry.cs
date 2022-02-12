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
using BPMS_DTOs.Header;

namespace BPMS_DAL.Repositories
{
    public class ServiceHeaderRepository : BaseRepository<ServiceHeaderEntity>
    {
        public ServiceHeaderRepository(BpmsDbContext context) : base(context) {}

        public Task<List<HeaderAllDTO>> All(Guid serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId)
                         .Select(x => new HeaderAllDTO 
                         {
                             Id = x.Id,
                             Name = x.Name,
                             Value = x.Value
                         })
                         .ToListAsync();
        }

        public Task<List<HeaderRequestDTO>> ForRequest(Guid serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId)
                         .Select(x => new HeaderRequestDTO 
                         {
                             Name = x.Name,
                             Value = x.Value
                         })
                         .ToListAsync();
        }
    }
}
