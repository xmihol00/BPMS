using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Role;
using BPMS_Common.Enums;
using BPMS_DTOs.User;

namespace BPMS_DAL.Repositories
{
    public class SolvingRoleRepository : BaseRepository<SolvingRoleEntity>
    {
        public SolvingRoleRepository(BpmsDbContext context) : base(context) {}

        public Task<List<RoleAllDTO>> AllNotInAgenda(Guid agendaId)
        {
            return _dbSet.Include(x => x.AgendaRoles)
                         .Where(x => x.AgendaRoles.All(y => y.AgendaId != agendaId))
                         .Select(x => new RoleAllDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description
                         })
                         .ToListAsync();
        }

        public Task<Guid> RoleByName(string name)
        {
            return _dbSet.Where(x => x.Name == name)
                         .Select(x => x.Id)
                         .FirstOrDefaultAsync();
        }
    }
}
