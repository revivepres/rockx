using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rockx.Data
{
    public class MockdbHandler : IDbHandler
    {
        private MockData _mockdata;

        public MockdbHandler()
        {
            _mockdata = new MockData();
        }

        public async Task<List<Person>> GetPeopleFromGroup(int groupId)
        {
            await Task.Delay(10);
            return _mockdata.People();
        }

        public async Task<List<DateTime>> GetDates()
        {
            await Task.Delay(10);
            return _mockdata.Dates();
        }

        public async Task<List<Person>> GetPeopleForDate(DateTime date)
        {
            await Task.Delay(10);
            return _mockdata.People();
        }

        public async Task<int> GetGuestsForDate(DateTime date)
        {
            await Task.Delay(10);
            return 7;
        }

        public async Task AddAttendance(List<Attendance> attendance, int guestCount)
        {
            await Task.Delay(10);
            return;
        }
    }
}
