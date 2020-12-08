using System;
using System.Collections.Generic;
using System.Text;

namespace SirenumTask1
{
    class payrate
    {
        public string name { get; set; } = "Default";
        public decimal hourlyRate { get; set; } = 10;
        public DateTime timeOfDayStart { get; set; } 
        public DateTime timeOfDayEnd { get; set; }
    }
}
