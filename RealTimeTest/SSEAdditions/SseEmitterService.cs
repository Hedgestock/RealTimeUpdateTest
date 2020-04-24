using Lib.AspNetCore.ServerSentEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTest.SSEAdditions
{
    public class SseEmitterService : ServerSentEventsService, ISseEmitterService
    {


        public SseEmitterService(IServerSentEventsService serverSentEventsService)
        {
            Program.data.DataChanged += (object sender, EventArgs e) =>
            {
                serverSentEventsService.SendEventAsync(Program.data.ToString());
            };
        }
    }
}
