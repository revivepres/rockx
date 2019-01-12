using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rockx.Data;

namespace rockx.Controllers
{
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private DbHandler _dbHandler;

        public ApiController()
        {
            var connectionString = @"Server=tcp:revivepresdev.database.windows.net,1433;Initial Catalog=ROCKDEV;Persist Security Info=False;User ID=rock;Password=r0ckxApp;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            _dbHandler = new DbHandler(connectionString);
        }

        [HttpGet("HasRecords/{date}")]
        public async Task<IActionResult> HasRecords(DateTime date)
        {
            if (ModelState.IsValid)
            {
                await Task.Delay(10);
                return Ok(true);
            }
            return BadRequest();
        }

        [HttpGet("GetRecords/{date}")]
        public async Task<IActionResult> GetRecords(DateTime date)
        {
            if (ModelState.IsValid)
            {
                var people = await _dbHandler.GetPeopleForDate(date);
                var guests = await _dbHandler.GetGuestsForDate(date);
                var result = new
                {
                    People = people,
                    GuestCount = guests

                };
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
