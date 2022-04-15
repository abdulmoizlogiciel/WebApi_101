using LS.WebSocketServer.Implementation;
using LS.WebSocketServer.Models;
using LS.WebSocketServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi_101.Utils;

namespace WebApi_101.Services
{
    public class TransactionalWebSocketHub : WebSocketHub
    {
        public TransactionalWebSocketHub(IMemoryCache memoryCache)
        {
        }

        public override async Task OnConnectedAsync(HttpContext context, SocketConnection socketConnection)
        {
            await base.OnConnectedAsync(context, socketConnection);
        }

        public override async Task OnDisconnectedAsync(SocketConnection socketConnection)
        {
            Console.WriteLine(socketConnection.Sub);
            await base.OnDisconnectedAsync(socketConnection);
        }
    }

    public class WebSocketUserIdProvider : IWebSocketUserIdProvider
    {
        public string GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.UserIdentifier();
        }
    }
}
