﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;

namespace RealTimeTest.SSEAdditions
{
    internal interface ISseEmitterService : IServerSentEventsService
    {
    }
}
