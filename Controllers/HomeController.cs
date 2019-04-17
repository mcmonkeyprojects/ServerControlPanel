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
            string bodyText = body.ReadToEnd();
            body.Close();
            int slash = bodyText.IndexOf('/');
            if (slash <= 0)
            {
                return BadRequest();
            }
            string sourceIP = Request.Headers["X-Forwarded-For"];
            if (FloodPrevention.ShouldDeny(sourceIP))
            {
                return Ok("flood_block/");
            }
            string command = bodyText.Substring(0, slash);
            string password = bodyText.Substring(slash + 1);
            if (command == "generate_hash")
            {
                return Ok("hash_response/" + UserValidator.Hash(password) + "/");
            }
            if (!UserValidator.CheckValidPassword(Program.PasswordHash, password))
            {
                FloodPrevention.NoteFlooding(sourceIP);
                return Ok("bad_password/");
            }
            switch (command)
            {
                case "status_check":
                    ProcessStartInfo procStart = new ProcessStartInfo("bash", "./config/status_check.sh")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };
                    Process statusProc = Process.Start(procStart);
                    StreamReader reader = statusProc.StandardOutput;
                    statusProc.WaitForExit();
                    string result = reader.ReadToEnd();
                    reader.Close();
                    if (result.ToLowerInvariant().Trim() == "true")
                    {
                        return Ok("status/online/");
                    }
                    return Ok("status/offline/");
                case "start":
                    Process startProc = Process.Start("bash", "./config/start.sh");
                    startProc.WaitForExit();
                    return Ok("success/");
                case "restart":
                    Process restartProc = Process.Start("bash", "./config/restart.sh");
                    restartProc.WaitForExit();
                    return Ok("success/");
                case "stop":
                    Process stopProc = Process.Start("bash", "./config/stop.sh");
                    stopProc.WaitForExit();
                    return Ok("success/");
                default:
                    return BadRequest();
            }
        }
    }
}
