using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rockx.Data;

namespace rockx.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private IDbHandler _dbHandler;

        public ApiController(IDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        [HttpGet("HasRecords/{date}")]
        public async Task<IActionResult> HasRecords(DateTime date)
        {
            if (ModelState.IsValid)
            {
                var people = await _dbHandler.GetPeopleForDate(date);
                var result = people.Count > 0 ? true : false;
                return Ok(result);
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
