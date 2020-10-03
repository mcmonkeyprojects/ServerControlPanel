using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerControlPanel.Models;

namespace ServerControlPanel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new StandardPageModel(Request));
        }

        public IActionResult HashGenerator()
        {
            return View(new StandardPageModel(Request));
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Post()
        {
            if (Request.Method != "POST")
            {
                return NotFound();
            }
            StreamReader body = new StreamReader(Request.Body);
            string bodyText = body.ReadToEndAsync().Result;
            body.Close();
            int slash = bodyText.IndexOf('/');
            if (slash <= 0)
            {
                return BadRequest();
            }
            string sourceIP = Request.Headers["X-Forwarded-For"];
            if (FloodPrevention.ShouldDeny(sourceIP))
            {
                Console.WriteLine("Flood blocked " + sourceIP);
                return Ok("flood_block/");
            }
            string command = bodyText.Substring(0, slash);
            string password = bodyText.Substring(slash + 1);
            if (command == "generate_hash")
            {
                Console.WriteLine("Generated has for " + sourceIP);
                return Ok("hash_response/" + UserValidator.Hash(password) + "/");
            }
            if (!UserValidator.CheckValidPassword(Program.PasswordHash, password))
            {
                FloodPrevention.NoteFlooding(sourceIP);
                Console.WriteLine($"Password failed for {sourceIP} with length {password.Length}");
                return Ok("bad_password/");
            }
            Console.WriteLine($"Accepting command {command} for {sourceIP}");
            switch (command)
            {
                case "status_check":
                    if (ServerLogicExecutor.StatusCheck())
                    {
                        return Ok("status/online/");
                    }
                    return Ok("status/offline/");
                case "start":
                    ServerLogicExecutor.Start();
                    return Ok("success/");
                case "restart":
                    ServerLogicExecutor.Restart();
                    return Ok("success/");
                case "stop":
                    ServerLogicExecutor.Stop();
                    return Ok("success/");
                default:
                    return BadRequest();
            }
        }
    }
}
