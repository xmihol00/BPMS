using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.User;
using BPMS_Common.Enums;

namespace BPMS_DAL.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>
    {
        public UserRepository(BpmsDbContext context) : base(context) {}

        public Task<List<UserIdNameDTO>> CreateModal()
        {
            return _dbSet.Include(x => x.Roles)
                         .Where(x => x.Roles.Any(y => y.Role == SystemRoleEnum.AgendaKeeper))
                         .Select(x => new UserIdNameDTO 
                         {
                             FullName = $"{x.Name} {x.Surname}",
                             Id = x.Id 
                         })
                         .ToListAsync();
        }

        public Task<List<UserIdNameDTO>> MissingInRole(Guid agendaId, Guid roleId)
        {
            return _dbSet.Include(x => x.UserRoles)
                         .Where(x => x.UserRoles.All(y => y.RoleId != roleId || y.AgendaId != agendaId))
                         .Select(x => new UserIdNameDTO
                         {
                             FullName = $"{x.Name} {x.Surname}",
                             Id = x.Id
                         })
                         .ToListAsync();
        }

        public Task<UserAuthDTO> Authenticate(string userName)
        {
            return _dbSet.Include(x => x.Roles)
                         .Where(x => x.UserName == userName)
                         .Select(x => new UserAuthDTO
                         {
                             FullName = $"{x.Name} {x.Surname}",
                             Id = x.Id,
                             Password = x.Password,
                             Roles = x.Roles.Select(y => y.Role).ToList()
                         })
                         .FirstAsync();
        }
    }
}
