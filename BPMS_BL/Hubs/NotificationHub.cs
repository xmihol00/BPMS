using System.Security.Claims;
using BPMS_BL.Facades;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Account;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Notification;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS_BL.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            if (Context.UserIdentifier != null)
            {
                Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            }

            return base.OnConnectedAsync();
        }

        public static async Task CreateSendNotifications(NotificationRepository notificationRepository, Guid targetId, NotificationTypeEnum type,
                                                         string info, Guid? currentUserId, params Guid[] userIds)
        {
            IHubContext<NotificationHub> hub = StaticData.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
            
            NotificationNewDTO notif = new NotificationNewDTO
            {
                Href = type.ToHref() + targetId,
                Info = info,
                State = NotificationStateEnum.Unread.ToString(),
                Text = type.ToText(),
                Date = DateTime.Now.ToString()
            };

            foreach (Guid userId in userIds.Where(x => x != currentUserId))
            {
                NotificationEntity notification = new NotificationEntity
                {
                    Date = DateTime.Now,
                    TargetId = targetId,
                    Type = type,
                    State = NotificationStateEnum.Unread,
                    UserId = userId,
                    Info = info
                };
                await notificationRepository.Create(notification);

                notif.Id = notification.Id;
                await hub.Clients.Group(notification.UserId.ToString()).SendAsync("Notification", notif);
            }
        }
    }
}
