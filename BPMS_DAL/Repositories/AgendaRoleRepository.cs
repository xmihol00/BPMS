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
using BPMS_DTOs.Agenda;

namespace BPMS_DAL.Repositories
{
    public class AgendaRoleRepository : BaseRepository<AgendaRoleEntity>
    {
        public AgendaRoleRepository(BpmsDbContext context) : base(context) {}

        public Task<List<AgendaRoleEntity>> ForRemoval(Guid agendaId, Guid roleId)
        {
            return _dbSet.Where(x => x.RoleId == roleId && x.AgendaId == agendaId)
                         .ToListAsync();
        }

        public Task<AgendaRoleEntity> RoleForRemoval(Guid id)
        {
            return _dbSet.Include(x => x.UserRoles)
                         .Include(x => x.Agenda)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<AgendaRoleEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<RoleDetailDTO>> Roles (Guid agendaId)
        {
            return _dbSet.Include(x => x.Role)
                         .Include(x => x.UserRoles)
                            .ThenInclude(x => x.User)
                         .Where(x => x.AgendaId == agendaId)
                         .Select(x => new RoleDetailDTO
                         {
                             Description = x.Role.Description,
                             Id = x.Id,
                             Name = x.Role.Name,
                             Users = x.UserRoles.Select(y => new UserIdNameDTO
                                                {
                                                    Id = y.UserId,
                                                    FullName = $"{y.User.Title} {y.User.Name} {y.User.Surname}",
                                                })
                                                .ToList()
                         })
                         .ToListAsync();
        }

        public Task<AgendaIdNameDTO> AgendaIdName(Guid id)
        {
            return _dbSet.Include(x => x.Agenda)
                         .Where(x => x.Id == id)
                         .Select(x => new AgendaIdNameDTO
                         {
                             Id = x.Agenda.Id,
                             Name = x.Agenda.Name
                         })
                         .FirstAsync();
        }
    }
}
