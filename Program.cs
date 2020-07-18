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
                PasswordHash = File.ReadAllText("./config/password_hash.txt").Replace("\r", "").Replace("\n", "");
                Console.WriteLine("Found password version " + PasswordHash[0..(PasswordHash.IndexOf(':'))]);
            }
            else
            {
                PasswordHash = "UNSET";
                Console.WriteLine("Warning: password not set");
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
