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
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Account;
using BPMS_DTOs.ConnectionRequest;
using BPMS_DTOs.Error;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class SystemFacade : BaseFacade
    {
        private readonly SystemRepository _systemRepository;
        private readonly ConnectionRequestRepository _connectionRequestRepository;
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private string _userName { get; set; } = string.Empty;

        public SystemFacade(SystemRepository systemRepository, ConnectionRequestRepository connectionRequestRepository, 
                            UserRepository userRepository, FilterRepository filterRepository, IMapper mapper)
        : base(filterRepository)
        {
            _systemRepository = systemRepository;
            _connectionRequestRepository = connectionRequestRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public void SetFilters(bool[] filters, Guid userId, string userName)
        {
            _systemRepository.Filters = filters;
            _systemRepository.UserId = userId;
            _userRepository.UserId = userId;
            _userId = userId;
            _userName = userName;
        }

        public async Task<List<SystemAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, _userId);
            _systemRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _systemRepository.All();
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
            _systemRepository.Entry(entity, x => 
            {
                x.Property(x => x.Key).IsModified = false;
                x.Property(x => x.State).IsModified = false;
            });

            await _systemRepository.Save();
            return await _systemRepository.InfoCard(dto.Id);
        }

        public async Task<SystemInfoCardDTO> Activate(SystemActivateDTO dto)
        {
            if(!PasswordHelper.Authenticate(await _userRepository.UserPassword(), dto.Password))
            {
                throw new Exception();
            }
            
            SystemEntity entity = await _systemRepository.BareAsync(dto.Id);
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.State = SystemStateEnum.Active;
            DstAddressDTO address = new DstAddressDTO
            {
                Key = entity.Key,
                SystemId = entity.Id,
                URL = StaticData.ThisSystemURL
            };
            if (!await CommunicationHelper.ActivateSystem(entity.URL, SymetricCipherHelper.JsonEncrypt(address), ""))
            {
                throw new Exception(); // TODO
            }

            await _systemRepository.Save();
            return await _systemRepository.InfoCard(dto.Id);
        }

        public Task<LastConnectionRequestDTO> ConnectionRequest(Guid id)
        {
            return _connectionRequestRepository.Last(id);
        }

        public async Task<Guid> Create(SystemCreateDTO dto)
        {
            SystemEntity entity = new SystemEntity
            {
                URL = StaticData.ThisSystemURL,
                State = SystemStateEnum.Inactive,
                Key = SymetricCipherHelper.NewKey(),
                ConnectionRequests = new List<ConnectionRequestEntity>
                {
                    new ConnectionRequestEntity
                    {
                        Date = DateTime.Now,
                        ForeignUserId = _userId,
                        SenderName = _userName,
                        Text = dto.Text,
                    }
                }
            };
            DstAddressDTO address = new DstAddressDTO
            {
                Key = StaticData.Key,
                SystemId = StaticData.ThisSystemId,
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
            entity.State = SystemStateEnum.Waiting;
            entity.ConnectionRequests.Clear();

            await _systemRepository.Create(entity);
            await _systemRepository.Save();
            return entity.Id;
        }
    }
}
