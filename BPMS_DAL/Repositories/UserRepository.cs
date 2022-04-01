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
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Workflow;

namespace BPMS_DAL.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>
    {
        public UserRepository(BpmsDbContext context) : base(context) {}

        public Task<List<UserIdNameDTO>> Create()
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .Where(x => x.SystemRoles.Any(y => y.Role == SystemRoleEnum.AgendaKeeper))
                         .Select(x => new UserIdNameDTO 
                         {
                             FullName = $"{x.Title} {x.Name} {x.Surname}",
                             Id = x.Id 
                         })
                         .ToListAsync();
        }

        public Task<List<UserIdNameDTO>> MissingInRole(Guid agendaId, Guid roleId)
        {
            return _dbSet.Include(x => x.UserRoles)
                            .ThenInclude(x => x.AgendaRole)
                         .Where(x => x.UserRoles.All(y => y.AgendaRole.Id != roleId))
                         .Select(x => new UserIdNameDTO
                         {
                             FullName = $"{x.Title} {x.Name} {x.Surname}",
                             Id = x.Id
                         })
                         .ToListAsync();
        }

        public Task<UserAuthDTO> Authenticate(string userName)
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .Include(x => x.Fitlers)
                         .Where(x => x.UserName == userName)
                         .Select(x => new UserAuthDTO
                         {
                             FullName = $"{x.Title} {x.Name} {x.Surname}",
                             Id = x.Id,
                             Password = x.Password,
                             Roles = x.SystemRoles.Select(y => y.Role).ToList(),
                             Filters = x.Fitlers.Select(x => x.Filter).ToList()
                         })
                         .FirstAsync();
        }

        public Task<UserEntity> BareRoles(string userName)
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .FirstAsync(x => x.UserName == userName);
        }

        public Task<UserInfoCardDTO> InfoCard(Guid id)
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .Select(x => new UserInfoCardDTO
                         {
                             Email = x.Email,
                             Id = x.Id,
                             Name = x.Name,
                             PhoneNumber = x.PhoneNumber,
                             Surname = x.Surname,
                             Roles = x.SystemRoles.Select(y => y.Role)
                                                        .ToList(),
                             SelectedUser = new UserAllDTO
                                            {
                                                FullName = $"{x.Title} {x.Name} {x.Surname}",
                                                Id = x.Id,
                                                Roles = x.SystemRoles.Select(y => y.Role)
                                                        .ToList()
                                            }
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<string?> UserPassword()
        {
            return _dbSet.Where(x => x.Id == UserId)
                         .Select(x => x.Password)
                         .FirstAsync();
        }

        public Task<List<UserIdNameDTO>> Keepers(SystemRoleEnum role)
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .Where(x => x.SystemRoles.Any(y => y.Role == role))
                         .Select(x => new UserIdNameDTO
                         {
                             FullName = $"{x.Title} {x.Name} {x.Surname}",
                             Id = x.Id
                         })
                         .ToListAsync();
        }

        public Task<List<UserAllDTO>> All(Guid? id = null)
        {
            IQueryable<UserEntity> query = _dbSet.Include(x => x.SystemRoles)
                                                 .Where(x => x.Id != id);
            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.UserAdmin)] || Filters[((int)FilterTypeEnum.UserAgendaKeeper)] ||
                    Filters[((int)FilterTypeEnum.UserWorkflowKeeper)] || Filters[((int)FilterTypeEnum.UserServiceKeeper)] ||
                    Filters[((int)FilterTypeEnum.UserModelKeeper)])
                {
                    query = query.Where(x => (Filters[((int)FilterTypeEnum.UserAdmin)] && x.SystemRoles.Any(y => y.Role == SystemRoleEnum.Admin)) ||
                                             (Filters[((int)FilterTypeEnum.UserAgendaKeeper)] && x.SystemRoles.Any(y => y.Role == SystemRoleEnum.AgendaKeeper)) ||
                                             (Filters[((int)FilterTypeEnum.UserWorkflowKeeper)] && x.SystemRoles.Any(y => y.Role == SystemRoleEnum.WorkflowKeeper)) ||
                                             (Filters[((int)FilterTypeEnum.UserServiceKeeper)] && x.SystemRoles.Any(y => y.Role == SystemRoleEnum.ServiceKeeper)) ||
                                             (Filters[((int)FilterTypeEnum.UserModelKeeper)] && x.SystemRoles.Any(y => y.Role == SystemRoleEnum.ModelKeeper)));
                }
            }
                         
            return query.Select(x => new UserAllDTO
                        {
                            FullName = $"{x.Title} {x.Name} {x.Surname}",
                            Id = x.Id,
                            Roles = x.SystemRoles.Select(x => x.Role).ToList()
                        })
                        .ToListAsync();
        }

        public Task<UserAllDTO> Selected(Guid id)
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .Select(x => new UserAllDTO
                         {
                             FullName = $"{x.Title} {x.Name} {x.Surname}",
                             Id = x.Id,
                             Roles = x.SystemRoles.Select(x => x.Role).ToList()
                         })
                         .FirstAsync(x => x.Id == id);
        }
        
        public Task<UserDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Agendas)
                            .ThenInclude(x => x.Models)
                         .Include(x => x.Agendas)
                            .ThenInclude(x => x.Systems)
                         .Include(x => x.Agendas)
                            .ThenInclude(x => x.Workflows)
                         .Include(x => x.Agendas)
                            .ThenInclude(x => x.AgendaRoles)
                                .ThenInclude(x => x.UserRoles)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Agenda)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Model)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Blocks)
                         .Include(x => x.SystemRoles)
                         .Select(x => new UserDetailDTO
                         {
                             Agendas = x.Agendas.Select(y => new AgendaAllDTO
                                                 {
                                                     Id = y.Id,
                                                     Name = y.Name,
                                                     ActiveWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Active).Count(),
                                                     PausedWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Paused || y.State == WorkflowStateEnum.Waiting).Count(),
                                                     FinishedWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Finished).Count(),
                                                     CanceledWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Canceled).Count(),
                                                     ModelsCount = y.Models.Count(),
                                                     SystemsCount = y.Systems.Count(),
                                                     UserCount = y.AgendaRoles.SelectMany(x => x.UserRoles).Count(),
                                                     MissingRolesCount = y.AgendaRoles.Where(y => y.UserRoles.Count == 0).Count(),
                                                 })
                                                 .ToList(),
                            Workflows = x.Workflows.Where(y => y.State == WorkflowStateEnum.Active)
                                                         .Select(y => new WorkflowAllDTO
                                                         {
                                                             AdministratorEmail = x.Email,
                                                             AdministratorName = $"{x.Title} {x.Name} {x.Surname}",
                                                             AgendaId = y.AgendaId,
                                                             AgendaName = y.Agenda.Name,
                                                             Description = y.Description,
                                                             Id = y.Id,
                                                             ModelId = y.ModelId,
                                                             ModelName = y.Model.Name,
                                                             Name = y.Name,
                                                             State = y.State,
                                                             SVG = y.Model.SVG
                                                         })
                                                         .ToList(),
                            Roles = x.SystemRoles.Select(y => y.Role)
                                                       .ToList(),
                            ActiveBlocks = x.Workflows.Where(y => y.State == WorkflowStateEnum.Active)
                                                      .Select(y => new WorkflowActiveBlocksDTO
                                                      {
                                                          Id = y.Id,
                                                          BlockIds = y.Blocks.Where(z => z.State == BlockWorkflowStateEnum.Active)
                                                                             .Select(z => z.BlockModelId)
                                                                             .ToList()
                                                      })
                                                      .ToList(),
                            Email = x.Email,
                            Id = x.Id,
                            Name = x.Name,
                            Surname = x.Surname,
                            UserName = x.UserName,
                            PhoneNumber = x.PhoneNumber,
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<UserEntity> Bare()
        {
            return _dbSet.FirstAsync(x => x.Id == UserId);
        }

        public Task<Guid[]> Admins()
        {
            return _dbSet.Include(x => x.SystemRoles)
                         .Where(x => x.SystemRoles.Any(y => y.Role == SystemRoleEnum.Admin))
                         .Select(x => x.Id)
                         .ToArrayAsync();
        }
    }
}
