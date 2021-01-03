using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerControlPanel.Models;

namespace ServerControlPanel
{
    public class Startup
    {
        public Startup()
        {
            if (File.Exists("config/config.txt"))
            {
                string[] config = File.ReadAllText("config/config.txt").Replace('\r', '\n').Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in config)
                {
                    string[] lineSplit = line.Split('=', 2);
                    string valueLow = lineSplit[1].Trim().ToLowerInvariant();
                    switch (lineSplit[0].ToLowerInvariant())
                    {
                        case "auto_check":
                            ServerSettings.AutoCheck = valueLow == "true";
                            break;
                        case "double_check":
                            ServerSettings.DoubleCheck = valueLow == "true";
                            break;
                        case "check_rate":
                            ServerSettings.CheckRateSeconds = Math.Max(1, int.Parse(valueLow));
                            break;
                    }
                }
                Console.WriteLine("Config loaded.");
            }
            ServerLogicExecutor.ShouldBeRunning = ServerLogicExecutor.StatusCheck();
            Console.WriteLine($"Is already running? {ServerLogicExecutor.ShouldBeRunning}");
            if (ServerSettings.AutoCheck)
            {
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Task.Delay(ServerSettings.CheckRateSeconds * 1000).Wait();
                        if (!ServerLogicExecutor.ShouldBeRunning)
                        {
                            continue;
                        }
                        bool isRunning = ServerLogicExecutor.StatusCheck();
                        if (isRunning)
                        {
                            continue;
                        }
                        if (ServerSettings.DoubleCheck)
                        {
                            Console.WriteLine("Server appears down, will double check...");
                            Task.Delay(ServerSettings.CheckRateSeconds * 1000).Wait();
                            if (!ServerLogicExecutor.ShouldBeRunning)
                            {
                                continue;
                            }
                            bool isRunningDoubleCheck = ServerLogicExecutor.StatusCheck();
                            if (isRunningDoubleCheck)
                            {
                                Console.WriteLine("Server seems fine, ignoring.");
                                continue;
                            }
                        }
                        Console.WriteLine("Server crashed! Restarting...");
                        ServerLogicExecutor.Start();
                    }
                });
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error/Code404"; 
                    await next();
                }
            });
            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
