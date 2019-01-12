using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rockx.Data;
using rockx.Models;

namespace rockx.Controllers
{
    public class ReviewController : Controller
    {
        private DbHandler _dbHandler;

        public ReviewController()
        {
            var connectionString = @"Server=tcp:revivepresdev.database.windows.net,1433;Initial Catalog=ROCKDEV;Persist Security Info=False;User ID=rock;Password=r0ckxApp;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            _dbHandler = new DbHandler(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            var model = new ReviewViewModel();
            model.Dates = await _dbHandler.GetDates();
            return View(model);
        }
    }
}