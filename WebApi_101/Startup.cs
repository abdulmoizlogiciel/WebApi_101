using LS.WebSocketServer;
using LS.WebSocketServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApi_101.Converters;
using WebApi_101.Middlewares;
using WebApi_101.SocketStuff;
using WebApi_101.Utils;

namespace WebApi_101
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services
            //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
            //    {
            //        options.ApiName = "weatherapi";
            //        options.Authority = Config.IdentityServerUrl;
            //    });


            //services.AddSingleton<ISomeService, SomeService>();
            services.AddTransient<ISomeService, SomeService>();

            services.AddMemoryCache();

            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
            });

            // with custom options
            services
                .AddWebSocket<TestHub>(options =>
                {
                    // set minimum frequency at which to send Ping/Pong keep-alive control frames
                    options.KeepAliveInterval = TimeSpan.FromSeconds(5);

                    // set max allowed protocol buffer size in kb which used to receive and parse frames
                    options.ReceiveBufferSize = 4;
                });
            // with specified authentication scheme
            //services
            //    .AddWebSocket<TestHub>()
            //    .AddAuthentication<WebSocketUserIDProvider>("myscheme");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            }

            app.UseMiddleware<RequestCultureMiddleware>();

            app.UseRouting();
            app.UseAuthentication(); // always use this before .UseAuthorization(); or u will get in a redirect-loop with identityserver.
            app.UseAuthorization();

            //app.MapWhen(x => x.WebSockets.IsWebSocketRequest && x.Request.Scheme.StartsWith("ws").Path.Equals("/serverws"), (app) =>
            //app.UseWebSockets(new Microsoft.AspNetCore.Builder.WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(5) });

            //app.MapWhen(x => x.WebSockets.IsWebSocketRequest && x.Request.Path.Equals("/ws"), (app) =>
            //app.MapWhen(x => x.WebSockets.IsWebSocketRequest && x.Request.Path.Equals("/ws"), (app) =>
            //{
            //    app.Run(async (context) =>
            //    {
            //        string webSocketKey = context.Request.Headers["Sec-WebSocket-Key"].ToString();
            //        string sub = context.User.UserIdentifier();

            //        WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();
            //        Console.WriteLine("AcceptWebSocketAsync");

            //        var socketConnection = new SocketConnection
            //        {
            //            ConnectionId = webSocketKey,
            //            Sub = sub,
            //            WebSocket = websocket,
            //        };
            //        try
            //        {
            //            while (socketConnection.WebSocket.State == WebSocketState.Open)
            //            {
            //                await ReceiverAsync(socketConnection);
            //            }
            //        }
            //        catch (WebSocketException ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //        Console.WriteLine("Connection broken");
            //    });
            //});

            app.UseWebSocketsWithPattern("/ws");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static async Task ReceiverAsync(SocketConnection socketConnection)
        {
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            Console.WriteLine("ReceiveAsync started");
            var receivedMessage = await socketConnection.WebSocket.ReceiveAsync(arraySegment, CancellationToken.None);
            Console.WriteLine("ReceiveAsync ended");
            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.Default.GetString(TrimEnd(arraySegment.Array));
                Console.WriteLine("Text message: " + message);
            }
            else if (receivedMessage.MessageType == WebSocketMessageType.Binary)
            {
                Console.WriteLine("Binary message");
            }
        }
        private static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);
            Array.Resize(ref array, lastIndex + 1);
            return array;
        }
    }
}
