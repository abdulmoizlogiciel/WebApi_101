using LS.WebSocketServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi_101.Utils;

namespace WebApi_101.SocketStuff
{
    public class WebSocketUserIdProvider : IWebSocketUserIdProvider
    {
        public string GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            string sub = claimsPrincipal?.FindFirst("sub")?.Value;
            return sub ?? claimsPrincipal?.FindFirst("client_id")?.Value;
        }
    }

    public class WebSocketUserIDProvider : IWebSocketUserIdProvider
    {
        public string GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.UserIdentifier();
        }
    }

}
