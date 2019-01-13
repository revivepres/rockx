using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rockx.Data;
using rockx.Models;

namespace rockx.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private IDbHandler _dbHandler;

        public ReviewController(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ReviewViewModel();
            model.Dates = await _dbHandler.GetDates();
            return View(model);
        }
    }
}