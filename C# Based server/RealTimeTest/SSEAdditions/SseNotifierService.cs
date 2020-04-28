using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;

namespace RealTimeTest.SSEAdditions
{
    public class SseNotifierService : ServerSentEventsService
    {
        public SseNotifierService()
        {
            Program.data.DataChanged += (object sender, EventArgs e) =>
            {
                this.SendEventAsync(Program.data.version.ToString());
            };
        }
    }
}
