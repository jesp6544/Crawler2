using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerLibrary.Models
{
    public class SlaveControl
    {
        // Contains latest succesful build number from appveyor
        public Version BuildNumber { get; set; }

        // True to kill all slaves
        public bool Stop { get; set; }

        // True to pause slaves
        public bool Pause { get; set; }
    }
}