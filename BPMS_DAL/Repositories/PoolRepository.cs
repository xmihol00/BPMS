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
using BPMS_Common;

namespace BPMS_DAL.Repositories
{
    public class PoolRepository : BaseRepository<PoolEntity>
    {
        public PoolRepository(BpmsDbContext context) : base(context) {} 

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

        public Task<List<PoolShareDTO>> Share(Guid modelId)
        {
            return _dbSet.Where(x => x.ModelId == modelId)
                            .Include(x => x.System)
                         .Select(x => new PoolShareDTO
                         {
                             Description = x.Description,
                             Id = x.Id,
                             ModelId = x.ModelId,
                             Name = x.Name,
                             SystemURL = x.System.URL
                         })
                         .ToListAsync();
        }

        public Task<List<PoolDstAddressDTO>> Addresses(Guid modelId)
        {
            return _dbSet.Include(x => x.System)
                         .Where(x => x.System.Id != StaticData.ThisSystemId && x.ModelId == modelId)
                         .Select(x => new PoolDstAddressDTO
                         {
                             Key = x.System.ObtainedKey,
                             PoolId = x.Id,
                             DestinationURL = x.System.URL
                         })
                         .ToListAsync();
        }

        public Task<List<Guid?>> AssignedSystems(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                            .ThenInclude(x => x.Pools)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Model.Pools)
                         .Where(x => x.Id != id)
                         .Select(x => x.SystemId)
                         .Where(x => x.HasValue)
                         .ToListAsync();
        }

        public Task<List<RoleAllDTO>> RolesOfAgenda(Guid poolId)
        {
            return _dbSet.Include(x => x.Model)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.AgendaRoles)
                                    .ThenInclude(x => x.Role)
                         .Where(x => x.Id == poolId)
                         .SelectMany(x => x.Model.Agenda.AgendaRoles)
                         .Select(x => x.Role)
                         .Select(x => new RoleAllDTO
                         {
                            Id = x.Id,
                            Name = x.Name
                         })
                         .Distinct()
                         .ToListAsync();
        }

        public Task<List<SystemPickerDTO>> OfAgenda(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.Systems)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Model.Agenda.Systems)
                         .Select(x => x.System)
                         .Select(x => new SystemPickerDTO
                          {
                              Name = x.Name,
                              Id = x.Id,
                              URL = x.URL
                          })
                          .ToListAsync();
        }
    }
}
