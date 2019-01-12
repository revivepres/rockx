using System;
using System.Collections.Generic;
using System.Linq;

namespace rockx.Data
{
    public class MockData
    {
        private static Random _random;

        public MockData()
        {
            _random = new Random();
        }

        public List<Person> People()
        {
            var result = new List<Person>();
            for (int i = 1; i <= 50; i++)
            {
                Person person = new Person();
                person.Id = i;
                person.FirstName = RandomString();
                person.LastName = RandomString();
                person.IsAttend = RandomBool();
                result.Add(person);
            }
            return result;
        }

        public List<DateTime> Dates()
        {
            var result = new List<DateTime>();
            for (int i = 1; i <= 20; i++)
            {
                result.Add(RandomDate());
            }
            return result;
        }

        private static string RandomString()
        {
            const string chars = "abcdefghijklmnopqrstuvwxy";
            int length = _random.Next(4, 7);
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static bool RandomBool()
        {
            return _random.Next(2) == 1;
        }

        private DateTime RandomDate()
        {
            DateTime startDate = new DateTime(2018, 1, 1);
            int range = (DateTime.Today - startDate).Days;
            return startDate.AddDays(_random.Next(range));
        }
    }
}
