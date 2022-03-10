using System.Security.Claims;
using BPMS_BL.Facades;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DTOs.Account;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Notification;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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

        public static async Task SendNotification(IHubClients clients, NotificationEntity notification)
        {
            NotificationNewDTO notif = new NotificationNewDTO
            {
                Id = notification.Id,
                Href = notification.Type.ToHref() + notification.TargetId,
                Info = notification.Info,
                State = notification.State.ToString(),
                Text = notification.Type.ToText()
            };
            await clients.Group(notification.UserId.ToString()).SendAsync("Notification", notif);
        }
    }
}
