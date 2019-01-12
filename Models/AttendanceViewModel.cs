using rockx.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rockx.Models
{
    public class AttendanceViewModel
    {
        public List<Person> People { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int GuestCount { get; set; }
    }
}
