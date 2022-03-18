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
    public class NotificationFacade : BaseFacade
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationFacade(FilterRepository filterRepository, NotificationRepository notificationRepository)
        : base(filterRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void SetFilters(bool[] filters)
        {
            _notificationRepository.Filters = filters;
            _notificationRepository.UserId = UserId;
        }

        public async Task<List<NotificationAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, UserId);
            _notificationRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _notificationRepository.All();
        }

        public async Task<List<NotificationAllDTO>> Seen(Guid id)
        {
            _notificationRepository.ChangeState(id, NotificationStateEnum.Read);
            await _notificationRepository.Save();
            return await _notificationRepository.All();
        }

        public async Task Mark(Guid id, bool marked)
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

        public async Task Remove(Guid id)
        {
            _notificationRepository.Remove(new NotificationEntity
            {
                Id = id
            });
            await _notificationRepository.Save();
        }

        public Task<List<NotificationAllDTO>> All()
        {
            return _notificationRepository.All(); 
        }
    }
}
