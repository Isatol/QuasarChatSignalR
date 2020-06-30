using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Hubs
{
    public class ChatHub : Hub
    {
        [Microsoft.AspNetCore.Authorization.Authorize()]
        public async Task SendMessage(string user, string message, string grupo)
        {             
            Console.WriteLine(Context.Items);
            List<Claim> claim = Context.User.Claims.ToList();            
            //claim.ForEach(c => 
            //{
            //    if (c.Type == "nombre" && c.Value == "Isaías") isIsa = true;
            //});
            //await Clients.Caller.SendAsync("Message", user, message, isIsa);            
            await Clients.Group(grupo).SendAsync("Message", user, message, Context.ConnectionId);
        }
        /// <summary>
        /// Agregar a un grupo en la primera conección
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {            
            Console.WriteLine(Clients);            
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Quitar el connectionID del grupo
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
