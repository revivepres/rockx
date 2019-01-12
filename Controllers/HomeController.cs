using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rockx.Data;
using rockx.Models;

namespace rockx.Controllers
{
    public class HomeController : Controller
    {
        private DbHandler _dbHandler;

        public HomeController()
        {
            var connectionString = @"Server=tcp:revivepresdev.database.windows.net,1433;Initial Catalog=ROCKDEV;Persist Security Info=False;User ID=rock;Password=r0ckxApp;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            _dbHandler = new DbHandler(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            var model = new AttendanceViewModel();
            model.People = await _dbHandler.GetPeople();
            model.GuestCount = 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(AttendanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Task.Delay(10);
                //return RedirectToAction(nameof(Index));
                return View();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
