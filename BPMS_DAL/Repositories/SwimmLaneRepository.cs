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
    }
}
