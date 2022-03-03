using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class SystemFacade
    {
        private readonly SystemRepository _systemRepository;
        private readonly IMapper _mapper;

        public SystemFacade(SystemRepository systemRepository, IMapper mapper)
        {
            _systemRepository = systemRepository;
            _mapper = mapper;
        }

        public Task<SystemDetailDTO> DetailPartial(Guid id)
        {
            return _systemRepository.Detail(id);
        }

        public async Task<SystemDetailDTO> Detail(Guid id)
        {
            SystemDetailDTO detail = await _systemRepository.Detail(id);
            detail.OtherSystems = await _systemRepository.All(id);
            detail.SelectedSystem = await _systemRepository.Selected(id);

            return detail;
        }

        public async Task<SystemOverviewDTO> Overview()
        {
            return new SystemOverviewDTO()
            {
                Systems = await _systemRepository.All()
            };
        }

        public async Task<SystemInfoCardDTO> Edit(SystemEditDTO dto)
        {
            SystemEntity entity = _mapper.Map<SystemEntity>(dto);
            _systemRepository.Update(entity);
            _systemRepository.Entry(entity, x => x.Property(x => x.Key).IsModified = false);

            await _systemRepository.Save();
            return await _systemRepository.InfoCard(dto.Id);
        }

        public async Task<Guid> Create(SystemCreateDTO dto)
        {
            SystemEntity entity = new SystemEntity
            {
                URL = StaticData.ThisSystemURL,
                Activity = SystemActivityEnum.InactiveAsked,
                Key = SymetricCipherHelper.NewKey()
            };
            PoolDstAddressDTO address = new PoolDstAddressDTO
            {
                Key = StaticData.Key,
                SystemId = entity.Id,
                URL = dto.URL
            };

            if (!await CommunicationHelper.CreateSystem(dto.URL, SymetricCipherHelper.JsonEncrypt(address), 
                                                                 JsonConvert.SerializeObject(entity)))
            {
                throw new Exception(); // TODO
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.URL = dto.URL;
            entity.Activity = SystemActivityEnum.Inactive;

            await _systemRepository.Create(entity);
            await _systemRepository.Save();
            return entity.Id;
        }
    }
}
