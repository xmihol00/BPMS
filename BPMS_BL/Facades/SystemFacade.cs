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
using BPMS_DAL;
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

        public void SetFilters(bool[] filters, string userName)
        {
            _systemRepository.Filters = filters;
            _systemRepository.UserId = UserId;
            _userRepository.UserId = UserId;
            _userName = userName;
        }

        public async Task<List<SystemAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, UserId);
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
            if (dto.URL.Last() != '/')
            {
                dto.URL += '/';
            }
            
            SystemEntity entity = await _systemRepository.BareAsync(dto.Id);
            DstAddressDTO address = new DstAddressDTO
            {
                DestinationURL = entity.URL,
                Encryption = entity.State == SystemStateEnum.ThisSystem ? EncryptionLevelEnum.Encrypted : entity.Encryption,
                SystemId = entity.Id,
                Key = entity.Key
            };

            if (entity.Encryption != dto.Encryption)
            {
                address.Encryption = entity.Encryption > entity.ForeignEncryption ? entity.Encryption : entity.ForeignEncryption;
                if (!await CommunicationHelper.ChangeEncryption(address, dto.Encryption, entity.ForeignEncryption))
                {
                    throw new Exception();
                }
            }

            entity.Encryption = entity.State == SystemStateEnum.ThisSystem ? EncryptionLevelEnum.Encrypted : dto.Encryption;
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.URL = dto.URL;

            await _systemRepository.Save();

            if (entity.Id == StaticData.ThisSystemId)
            {
                StaticData.ThisSystemURL = entity.URL;
            }

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
                DestinationURL = entity.URL,
                Encryption = entity.Encryption > entity.ForeignEncryption ? entity.Encryption : entity.ForeignEncryption
            };

            if (!await CommunicationHelper.ActivateSystem(address))
            {
                throw new Exception();
            }

            await _systemRepository.Save();
            return await _systemRepository.InfoCard(dto.Id);
        }

        public async Task<SystemInfoCardDTO> Reactivate(SystemReactivateDTO dto)
        {
            SystemEntity entity = await _systemRepository.BareAsync(dto.Id);
            entity.State = SystemStateEnum.Waiting;
            DstAddressDTO address = new DstAddressDTO
            {
                Key = entity.Key,
                SystemId = entity.Id,
                DestinationURL = entity.URL,
                Encryption = entity.Encryption > entity.ForeignEncryption ? entity.Encryption : entity.ForeignEncryption
            };
            ConnectionRequestEntity request = new ConnectionRequestEntity
            {
                Date = DateTime.Now,
                ForeignUserId = UserId,
                SystemId = dto.Id,
                SenderName = _userName,
                Text = dto.Text
            };

            if (!await CommunicationHelper.ReactivateSystem(address, request))
            {
                throw new Exception();
            }

            await _systemRepository.Save();
            return await _systemRepository.InfoCard(dto.Id);
        }

        public async Task<SystemInfoCardDTO> Deactivate(Guid id)
        {
            SystemEntity entity = await _systemRepository.BareAsync(id);
            entity.State = SystemStateEnum.Deactivated;
            DstAddressDTO address = new DstAddressDTO
            {
                Key = entity.Key,
                SystemId = entity.Id,
                DestinationURL = entity.URL,
                Encryption = entity.Encryption > entity.ForeignEncryption ? entity.Encryption : entity.ForeignEncryption
            };

            if (!await CommunicationHelper.DeactivateSystem(address))
            {
                throw new Exception();
            }

            await _systemRepository.Save();
            return await _systemRepository.InfoCard(id);
        }

        public Task<LastConnectionRequestDTO> ConnectionRequest(Guid id)
        {
            return _connectionRequestRepository.Last(id);
        }

        public async Task<Guid> Create(SystemCreateDTO dto)
        {
            if (dto.URL.Last() != '/')
            {
                dto.URL += '/';
            }

            UserEntity user = await _userRepository.Bare();
            SystemEntity entity = new SystemEntity
            {
                URL = StaticData.ThisSystemURL,
                State = SystemStateEnum.Inactive,
                Key = await SymetricCryptoHelper.NewKey(),
                ConnectionRequests = new List<ConnectionRequestEntity>
                {
                    new ConnectionRequestEntity
                    {
                        Date = DateTime.Now,
                        ForeignUserId = UserId,
                        SenderName = user.Name,
                        Text = dto.Text,
                        SenderEmail = user.Email,
                        SenderPhone = user.PhoneNumber
                    }
                },
                Encryption = dto.Encryption,
                ForeignEncryption = dto.Encryption
            };
            DstAddressDTO address = new DstAddressDTO
            {
                Key = await _systemRepository.ThisSystemKey(),
                SystemId = StaticData.ThisSystemId,
                DestinationURL = dto.URL,
                Encryption = EncryptionLevelEnum.Encrypted
            };

            if (!await CommunicationHelper.CreateSystem(address, entity))
            {
                throw new Exception();
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
