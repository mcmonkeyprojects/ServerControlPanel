using System;
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
