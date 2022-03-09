using System.Security.Claims;
using BPMS_BL.Facades;
using BPMS_DTOs.Account;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BPMS.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext().User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            Groups.AddToGroupAsync(Context.ConnectionId, userId);

            return base.OnConnectedAsync();
        }
    }
}
