using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.System;

namespace BPMS_DAL.Repositories
{
    public class SystemRepository : BaseRepository<SystemEntity>
    {
        public SystemRepository(BpmsDbContext context) : base(context) {} 

        public Task<List<SystemPickerDTO>> SystemsOfAgenda(Guid? agendaId)
        {
            return _dbSet.Include(x => x.Agendas)
                         .Where(x => x.Agendas.Any(y => y.AgendaId == agendaId))
                         .Select(x => new SystemPickerDTO
                         {
                             SystemId = x.Id,
                             Value = x.Name
                         })
                         .ToListAsync();
        }

        public Task<List<SystemIdNameDTO>> IdNames()
        {
            return _dbSet.Select(x => new SystemIdNameDTO
                         {
                             Name = x.Name,
                             Id = x.Id
                         })
                         .ToListAsync();
        }

        public Task<Guid> IdFromUrl(string systemURL)
        {
            return _dbSet.Where(x => x.URL == systemURL)
                         .Select(x => x.Id)
                         .FirstOrDefaultAsync();
        }
    }
}
