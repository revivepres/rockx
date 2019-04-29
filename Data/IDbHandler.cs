using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rockx.Data
{
    public interface IDbHandler
    {
        Task<List<Person>> GetPeopleFromGroup(int groupId);
        Task<List<DateTime>> GetDates();
        Task<List<Person>> GetPeopleFromGroupByDate(int groupId, DateTime date);
        Task<int> GetGuestsForDate(DateTime date);
        Task AddGuestAttendance(int guestCount, DateTime date, int personId);
        Task AddAttendance(List<Attendance> attendance, int guestCount);
    }
}
