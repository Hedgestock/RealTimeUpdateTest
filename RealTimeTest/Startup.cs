using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            // Register default ServerSentEventsService.
            services.AddServerSentEvents();

            services.AddServerSentEvents<ISseEmitterService, SseEmitterService>();


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
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapServerSentEvents("/see-emitter");
                    endpoints.MapPost("/update-receiver", async context =>
                    {
                        var request = context.Request;
                        var stream = new StreamReader(request.Body);
                        var body = await stream.ReadToEndAsync();
                        Program.data.data = JsonConvert.DeserializeObject<Program.DataHolder>(body).data;
                        context.Response.StatusCode = 200 ;
                        await context.Response.CompleteAsync();
                    });
                    endpoints.MapGet("/data",
                        async context => { await context.Response.WriteAsync(Program.data.ToString()); });
                });

        }
    }
}
