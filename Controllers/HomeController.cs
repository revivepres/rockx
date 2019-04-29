using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using rockx.Data;
using rockx.Models;

namespace rockx.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IDbHandler _dbHandler;
        private IConfiguration _configuration;

        public HomeController(IDbHandler dbHandler, IConfiguration configuration)
        {
            _dbHandler = dbHandler;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AttendanceViewModel();
            var groupid = _configuration.GetValue<int>("GroupId");
            model.People = await _dbHandler.GetPeopleFromGroup(groupid);
            model.GuestCount = 0;
            model.Date = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(AttendanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Only save guest could
                int personid = _configuration.GetValue<int>("PersonAliasId");
                await _dbHandler.AddGuestAttendance(model.GuestCount, model.Date, personid);
                return View("Index", model);

                //List<Attendance> attendance = new List<Attendance>();
                //foreach (var person in model.People)
                //{
                //    Attendance record = new Attendance
                //    {
                //        CampusId = _configuration.GetValue<int>("CampusId"),
                //        CreatedByPersonAliasId = _configuration.GetValue<int>("PersonAliasId"),
                //        CreatedDateTime = DateTime.Now,
                //        DidAttend = person.IsAttend,
                //        Guid = Guid.NewGuid(),
                //        GroupId = _configuration.GetValue<int>("GroupId"),
                //        LocationId = _configuration.GetValue<int>("LocationId"),
                //        ModifiedByPersonAliasId = _configuration.GetValue<int>("PersonAliasId"),
                //        ModifiedDateTime = DateTime.Now,
                //        Note = "Recorded from Rockx",
                //        PersonAliasId = person.Id,
                //        Rsvp = _configuration.GetValue<int>("Rsvp"),
                //        ScheduleId = _configuration.GetValue<int>("ScheduleId"),
                //        StartDateTime = model.Date
                //    };
                //    attendance.Add(record);
                //}
                //await _dbHandler.AddAttendance(attendance, model.GuestCount);
                //return View("Index", model);
            }
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
