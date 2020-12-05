using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureKestrel(o =>
                    {
                        ipConfig(o);
                    });
                });

        private static void ipConfig(KestrelServerOptions o)
        {
            var portStr = CmdHelper.QueryParameter("-p", "8080");
            if (!int.TryParse(portStr, out int port)) { port = 9001; }
            var ip = CmdHelper.QueryParameter("-ip", "localhost");
            Console.WriteLine(ip);
            if (ip.ToLower() == "any")
            {
                Console.WriteLine("ListenAnyIP");
                o.ListenAnyIP(port);
            }
            else if (ip == "127.0.0.1" || ip.ToLower() == "localhost")
            {
                Console.WriteLine("ListenLocalhost");
                o.ListenLocalhost(port);
            }
            else
            {
                Console.WriteLine("Listen");
                o.Listen(System.Net.IPAddress.Parse(ip), port);
            }
        }

    }
}
