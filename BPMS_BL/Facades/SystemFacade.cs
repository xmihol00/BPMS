using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
using Microsoft.AspNetCore.Authentication;

namespace BPMS_BL.Facades
{
    public class SystemFacade
    {
        private readonly SystemRepository _systemRepository;

        public SystemFacade(SystemRepository systemRepository)
        {
            _systemRepository = systemRepository;
        }

        public async Task<SystemOverviewDTO> Overview()
        {
            return new SystemOverviewDTO()
            {
                Systems = await _systemRepository.All()
            };
        }
    }
}
