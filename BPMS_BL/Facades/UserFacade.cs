using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Account;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using BPMS_BL.Helpers;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Notification;

namespace BPMS_BL.Facades
{
    public class UserFacade : BaseFacade
    {
        private readonly UserRepository _userRepository;
        private readonly SystemRoleRepository _systemRoleRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public UserFacade(UserRepository userRepository, SystemRoleRepository systemRoleRepository, FilterRepository filterRepository,
                          NotificationRepository notificationRepository, IMapper mapper)
        : base(filterRepository)
        {
            _userRepository = userRepository;
            _systemRoleRepository = systemRoleRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public void SetFilters(bool[] filters, Guid userId)
        {
            _userRepository.Filters = filters;
            _userRepository.UserId = userId;
            _notificationRepository.Filters = filters;
            _notificationRepository.UserId = userId;
            _userId = userId;
        }

        public async Task<List<UserAllDTO>> FilterUsers(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, _userId);
            _userRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _userRepository.All();
        }

        public async Task<List<NotificationAllDTO>> FilterNotifications(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, _userId);
            _userRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _notificationRepository.All();
        }

        public async Task<(ClaimsPrincipal principal, AuthenticationProperties authProperties)> Authenticate(string userName, string password, HttpResponse response)
        {
            UserAuthDTO user = await _userRepository.Authenticate(userName);
            if (PasswordHelper.Authenticate(user.Password, password))
            {
                foreach (FilterTypeEnum value in Enum.GetValues<FilterTypeEnum>())
                {
                    CookieHelper.SetCookie(value, !user.Filters.Contains(value), response);
                }
                return ClaimsAndProperties(user);
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<(ClaimsPrincipal, AuthenticationProperties)> Create(AccountCreateDTO dto)
        {
            UserEntity entity = await _userRepository.BareRoles(dto.UserName);
            if (entity.Password != null || dto.Password != dto.Password)
            {
                throw new Exception();
            }
            entity.Password = PasswordHelper.HashPassword(dto.Password);
            await _userRepository.Save();

            return ClaimsAndProperties(_mapper.Map<UserAuthDTO>(entity));
        }

        public async Task<Guid> Create(UserCreateEditDTO dto)
        {
            UserEntity entity = _mapper.Map<UserEntity>(dto);

            foreach (SystemRoleEnum role in dto.AddedRoles)
            {
                entity.SystemRoles.Add(new SystemRoleEntity
                {
                    Role = role
                });
            }

            await _userRepository.Create(entity);
            await _userRepository.Save();
            return entity.Id;
        }

        public async Task NotificationSeen(Guid id)
        {
            _notificationRepository.ChangeState(id, NotificationStateEnum.Read);
            await _notificationRepository.Save();
        }

        public async Task NotificationMark(Guid id, bool marked)
        {
            if (marked)
            {
                _notificationRepository.ChangeState(id, NotificationStateEnum.Read);
            }
            else
            {
                _notificationRepository.ChangeState(id, NotificationStateEnum.Marked);
            }
            await _notificationRepository.Save();
        }

        public Task<List<NotificationAllDTO>> AllNotifications()
        {
            return _notificationRepository.All(); 
        }

        public async Task<UserInfoCardDTO> Edit(UserCreateEditDTO dto)
        {
            UserEntity entity = _mapper.Map<UserEntity>(dto);
            _userRepository.Update(entity);
            _userRepository.Entry(entity, x => x.Property(x => x.Password).IsModified = false);

            foreach (SystemRoleEnum role in dto.RemovedRoles)
            {
                _systemRoleRepository.Remove(new SystemRoleEntity
                {
                    Role = role,
                    UserId = dto.Id
                });
            }
            foreach (SystemRoleEnum role in dto.AddedRoles)
            {
                await _systemRoleRepository.Create(new SystemRoleEntity
                {
                    Role = role,
                    UserId = dto.Id
                });
            }

            await _userRepository.Save();
            return await _userRepository.InfoCard(dto.Id); 
        }

        private (ClaimsPrincipal, AuthenticationProperties) ClaimsAndProperties(UserAuthDTO user) 
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
            };

            foreach (SystemRoleEnum role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "user");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            AuthenticationProperties authProperties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddHours(3),
                IsPersistent = true,
            };

            return (principal, authProperties);
        }

        public async Task<UserOverviewDTO> Overview()
        {
            return new UserOverviewDTO
            {
                Users = await _userRepository.All()
            };
        }

        public Task<UserDetailDTO> DetailPartial(Guid id)
        {
            return _userRepository.Detail(id);
        }

        public async Task<UserDetailDTO> Detail(Guid id)
        {
            UserDetailDTO detail = await DetailPartial(id);
            detail.OtherUsers = await _userRepository.All(id);
            detail.SelectedUser = await _userRepository.Selected(id);

            return detail;
        }
    }
}
