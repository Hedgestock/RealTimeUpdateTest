using Lib.AspNetCore.ServerSentEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTest.SSEAdditions
{
    public class SseEmitterService : ServerSentEventsService, ISseEmitterService
    {

        private readonly IServerSentEventsService _serverSentEventsService;

        public SseEmitterService(IServerSentEventsService serverSentEventsService)
        {
            _serverSentEventsService = serverSentEventsService;
            Program.data.DataChanged += (object sender, EventArgs e) =>
            {
                _serverSentEventsService.SendEventAsync(Program.data.ToString());
            };
        }
    }
}
