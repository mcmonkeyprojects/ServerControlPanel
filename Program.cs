using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ServerControlPanel
{
    public class Program
    {
        public static string PasswordHash;

        public static void Main(string[] args)
        {
            if (File.Exists("./config/password_hash.txt"))
            {
                PasswordHash = File.ReadAllText("./config/password_hash.txt");
            }
            else
            {
                PasswordHash = "UNSET";
            }
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
