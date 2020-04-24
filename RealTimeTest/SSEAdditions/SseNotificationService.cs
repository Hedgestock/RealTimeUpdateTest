using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;

namespace RealTimeTest.SSEAdditions
{
    public class SseNotificationService : ServerSentEventsService, ISseNotificationService
    {
    }
}
