using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace UniverseScale.Core
{
    public class TimeScale
    {
        public const int Seconds = 60;
        public const int Hours = Seconds * 60;
        public const int Days = Hours * 24;
        public const int Years = Days * 365;

        public static decimal RadianPeriod(float Period)
        {
            if(Period == 0)
            {
                return 0;
            }
            return (decimal)(Math.PI * 2 / Period);
        }
    }
}