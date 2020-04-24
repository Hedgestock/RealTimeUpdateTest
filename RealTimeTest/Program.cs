using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RealTimeTest
{
    public class Program
    {

        public class DataHolder
        {
            private string _data = "";
            private int _version = 0;

            public string data
            {
                get { return _data;}
                set
                {
                    _data = value;
                    _version++;
                    DataChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public int version { get { return _version; } }

            public event EventHandler DataChanged;

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
        public static DataHolder data = new DataHolder();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
