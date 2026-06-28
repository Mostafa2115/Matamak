
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class OrderHub:Hub 
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;

            if (user?.IsInRole("Cashier") == true || user?.IsInRole("Admin") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Cashiers");
            }

            // Use username to match "User_" + CustomerName in order broadcasts
            var username = user?.FindFirst(ClaimTypes.Name)?.Value 
                           ?? user?.FindFirst("unique_name")?.Value 
                           ?? user?.FindFirst("name")?.Value 
                           ?? Context.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "User_" + username);
            }

            await base.OnConnectedAsync();
        }
    }
}
