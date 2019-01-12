using System;

namespace rockx.Data
{
    public class Attendance
    {
        public int LocationId { get; set; }
        public int ScheduleId { get; set; }
        public int GroupId { get; set; }
        public DateTime StartDateTime { get; set; }
        public bool DidAttend { get; set; }
        public string Note { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int CreatedByPersonAliasId { get; set; }
        public int ModifiedByPersonAliasId { get; set; }
        public int CampusId { get; set; }
        public int PersonAliasId { get; set; }
        public int Rsvp { get; set; }
    }
}
