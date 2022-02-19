using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockModel.ConfigTypes;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.System;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Authentication;

namespace BPMS_BL.Facades
{
    public class UserFacade
    {
        private readonly UserRepository _userRepository;

        public UserFacade(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(ClaimsPrincipal principal, AuthenticationProperties authProperties)> Authenticate(string userName, string password)
        {
            UserAuthDTO user = await _userRepository.Authenticate(userName);
            if (PasswordHelper.Authenticate(user.Password, password))
            {
                return ClaimsAndProperties(user);
            }
            else
            {
                throw new Exception();
            }
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
                ExpiresUtc = DateTimeOffset.Now.AddHours(1),
                IsPersistent = true,
            };

            return (principal, authProperties);
        }
    }
}
