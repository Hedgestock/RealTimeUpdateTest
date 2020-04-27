using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RealTimeTest.SSEAdditions;


namespace RealTimeTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServerSentEvents<IServerSentEventsService, SseEmitterService>();
            services.AddServerSentEvents<IServerSentEventsService, SseNotifierService>();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/event-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapServerSentEvents<SseEmitterService>("/see-emitter");
                    endpoints.MapServerSentEvents<SseNotifierService>("/see-notifier");
                    endpoints.MapPost("/update-receiver", async context =>
                    {
                        var request = context.Request;
                        var stream = new StreamReader(request.Body);
                        var body = await stream.ReadToEndAsync();
                        Program.data.data = JsonConvert.DeserializeObject<Program.DataHolder>(body).data;
                        context.Response.StatusCode = 200;
                        await context.Response.CompleteAsync();
                    });
                    endpoints.MapGet("/data",
                        async context => { await context.Response.WriteAsync(Program.data.ToString()); });
                })
                .Use(async (context, next) =>
                {
                    if (context.Request.Path == "/ws-emitter-receiver")
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await Echo(webSocket);
                            return;
                        }

                        context.Response.StatusCode = 400;
                        return;

                    }
                    await next();

                });

        }

        private async Task Echo(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            async void onChangeHandler(object sender, EventArgs e)
            {
                var str = Program.data.ToString();
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(str), 0, str.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            };
            Program.data.DataChanged += onChangeHandler;
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                Program.data.data = Encoding.Default.GetString(buffer).Replace("\0", string.Empty); ;
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            Program.data.DataChanged -= onChangeHandler;
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }


    }
}
