using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace RealTimeTest.SSEAdditions
{
    public class EmitterService : BackgroundService
    {

        private readonly IServerSentEventsService _serverSentEventsService;

        public EmitterService(IServerSentEventsService serverSentEventsService)
        {
            _serverSentEventsService = serverSentEventsService;
            Program.data.DataChanged += (object sender, EventArgs e) =>
            {
                _serverSentEventsService.SendEventAsync(JsonConvert.SerializeObject(Program.data));
                Debug.WriteLine("Data sent");
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }

    }
}