using Lib.AspNetCore.ServerSentEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTest.SSEAdditions
{
    public class SseEmitterService : ServerSentEventsService
    {
        public SseEmitterService()
        {
            Program.data.DataChanged += (object sender, EventArgs e) =>
            {
                this.SendEventAsync(Program.data.ToString());
            };
        }
    }
}
