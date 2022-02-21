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

namespace BPMS_DAL.Repositories
{
    public class UserRoleRepository : BaseRepository<UserRoleEntity>
    {
        public UserRoleRepository(BpmsDbContext context) : base(context) {}

        public Task<UserRoleEntity> RoleForRemoval(Guid userId, Guid agendaRoleId)
        {
            return _dbSet.FirstAsync(x => x.AgendaRoleId == agendaRoleId && x.UserId == userId);
        }
    }
}
