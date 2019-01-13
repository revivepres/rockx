using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using rockx.Data;

namespace rockx.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private IDbHandler _dbHandler;
        private IConfiguration _configuration;

        public ApiController(IDbHandler dbHandler, IConfiguration configuration)
        {
            _dbHandler = dbHandler;
            _configuration = configuration;
        }

        [HttpGet("HasRecords/{date}")]
        public async Task<IActionResult> HasRecords(DateTime date)
        {
            if (ModelState.IsValid)
            {
                var groupid = _configuration.GetValue<int>("GroupId");
                var people = await _dbHandler.GetPeopleFromGroupByDate(groupid, date);
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
                var groupid = _configuration.GetValue<int>("GroupId");
                var people = await _dbHandler.GetPeopleFromGroupByDate(groupid, date);
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
