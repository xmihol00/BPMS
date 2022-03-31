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
using BPMS_DTOs.Lane;
using BPMS_DTOs.Role;

namespace BPMS_DAL.Repositories
{
    public class LaneRepository : BaseRepository<LaneEntity>
    {
        public LaneRepository(BpmsDbContext context) : base(context) {}

        public Task<List<LaneEntity>> ForRoleUnset(Guid agendaId, Guid roleId)
        {
            return _dbSet.Include(x => x.Pool)
                            .ThenInclude(x => x.Model)
                         .Where(x => x.RoleId == x.RoleId && x.Pool.Model.AgendaId == agendaId)
                         .ToListAsync();
        }

        public Task<LaneConfigDTO> Config(Guid id)
        {
            return _dbSet.Include(x => x.Pool)
                            .ThenInclude(x => x.Model)
                                .ThenInclude(x => x.Agenda)
                                    .ThenInclude(x => x.AgendaRoles)
                                        .ThenInclude(x => x.Role)
                         .Select(x => new LaneConfigDTO
                         {
                             CurrentRoleId = x.RoleId,
                             Id = x.Id,
                             Name = x.Name,
                             Roles = x.Pool.Model.Agenda.AgendaRoles
                                      .Select(y => y.Role)
                                      .Select(y => new RoleAllDTO
                                      {
                                          Id = y.Id,
                                          Description = y.Description,
                                          Name = y.Name
                                      })
                                      .ToList()
                         })
                         .FirstAsync(x => x.Id == id);

        }

        public Task<LaneEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}
