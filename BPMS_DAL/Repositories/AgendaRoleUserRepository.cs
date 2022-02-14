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

namespace BPMS_DAL.Repositories
{
    public class AgendaRoleUserRepository : BaseRepository<AgendaRoleUserEntity>
    {
        public AgendaRoleUserRepository(BpmsDbContext context) : base(context) {}

        public Task<List<AgendaRoleUserEntity>> ForRemoval(Guid agendaId, Guid roleId)
        {
            return _dbSet.Where(x => x.RoleId == roleId && x.AgendaId == agendaId)
                         .ToListAsync();
        }

        public Task<AgendaRoleUserEntity> RoleForRemoval(Guid userId, Guid agendaId, Guid roleId)
        {
            return _dbSet.Where(x => x.UserId == userId && x.RoleId == roleId && x.AgendaId == agendaId)
                         .FirstAsync();
        }

        public Task<AgendaRoleUserEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}
