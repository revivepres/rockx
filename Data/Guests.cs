using System;

namespace rockx.Data
{
    public class Guests
    {
        public DateTime CreatedDateTime { get; set; }
        public DateTime SundayDateTime { get; set; }
        public int Count { get; set; }
        public int CreatedByPersonAliasId { get; set; }
    }
}
