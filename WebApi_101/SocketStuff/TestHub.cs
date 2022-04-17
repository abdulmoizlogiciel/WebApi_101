using LS.WebSocketServer.Implementation;
using LS.WebSocketServer.Models;
using LS.WebSocketServer.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi_101.SocketStuff
{
    public class TestHub : WebSocketHub
    {
        public override Task OnConnectedAsync(HttpContext context, SocketConnection socketConnection)
        {
            string msg = "WebSocket connected. Internal service port: " + context.Request.Host.Port;
            string msgSerialized = JsonConvert.SerializeObject(new { msg });

            byte[] bytes = Encoding.Default.GetBytes(msgSerialized);
            var arraySegment = new ArraySegment<byte>(bytes);
            socketConnection.WebSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

            return base.OnConnectedAsync(context, socketConnection);
        }

        public override Task OnDisconnectedAsync(SocketConnection socketConnection)
        {
            return base.OnDisconnectedAsync(socketConnection);
        }

        public override Task OnMessageReceivedAsync(SocketConnection socketConnection, string message)
        {
            return base.OnMessageReceivedAsync(socketConnection, message);
        }

        public override Task OnMessageReceivedAsync(SocketConnection socketConnection, byte[] bytes)
        {
            return base.OnMessageReceivedAsync(socketConnection, bytes);
        }
    }
}
