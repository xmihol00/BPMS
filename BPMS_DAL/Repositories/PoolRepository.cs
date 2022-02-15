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
using BPMS_DTOs.Role;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Repositories
{
    public class PoolRepository : BaseRepository<PoolEntity>
    {
        public PoolRepository(BpmsDbContext context) : base(context) {} 

        public Task<SystemPickerDTO?> CurrentSystem(Guid id)
        {
            return _dbSet.Include(x => x.System)
                         .Select(x => new SystemPickerDTO
                         {
                            Value = x.System.Name,
                            SystemId = x.System.Id
                         })
                         .FirstOrDefaultAsync(x => x.SystemId == id);
        }

        public Task<PoolEntity> DetailForEdit(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                            .ThenInclude(x => x.Pools)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<PoolConfigDTO> Config(Guid id)
        {
            return _dbSet.Select(x => new PoolConfigDTO
                         {
                             SystemId = x.SystemId,
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<SystemIdAgendaIdDTO> CurrentSystemIdAgendaId(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                         .Where(x => x.Id == id)
                         .Select(x => new SystemIdAgendaIdDTO
                         {
                             AgendaId = x.Model.AgendaId,
                             SystemId = x.SystemId
                         })
                         .FirstAsync();
        }

        public Task<List<RoleAllDTO>> RolesOfAgenda(Guid poolId)
        {
            return _dbSet.Include(x => x.Model)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.UserRoles)
                                    .ThenInclude(x => x.Role)
                         .Where(x => x.Id == poolId)
                         .SelectMany(x => x.Model.Agenda.UserRoles)
                         .Select(x => x.Role)
                         .Select(x => new RoleAllDTO
                         {
                            Id = x.Id,
                            Name = x.Name
                         })
                         .Distinct()
                         .ToListAsync();
        }
    }
}
