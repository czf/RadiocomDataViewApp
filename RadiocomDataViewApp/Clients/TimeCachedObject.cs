using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadiocomDataViewApp.Clients
{
    public class TimeCachedObject<T>
    {
        private const int UPDATE_HOUR_INTERVAL = 4;
        public DateTimeOffset NextUpdateHour { get; set; }
        public T CachedObject { get; set; }

        public static DateTimeOffset CalculateNextUpdateHour()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            int hourDiff = now.Hour % UPDATE_HOUR_INTERVAL;
            DateTimeOffset nextUpdateHour = now.AddHours(UPDATE_HOUR_INTERVAL - hourDiff).Subtract(TimeSpan.FromMinutes(now.Minute)).Subtract(TimeSpan.FromSeconds(now.Second));
            return nextUpdateHour;
        }
    }
}
