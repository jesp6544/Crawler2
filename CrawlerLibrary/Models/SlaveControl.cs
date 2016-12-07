using System;

namespace CrawlerLibrary.Models
{
    public class SlaveControl
    {
        public long Id { get; set; }

        // Contains latest succesful build number from appveyor
        public int BuildNumber { get; set; }

        public DateTime TimeStamp { get; set; }

        // True to kill all slaves
        public bool? Stop { get; set; }

        // True to pause slaves
        public bool? Pause { get; set; }
    }
}