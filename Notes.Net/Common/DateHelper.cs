using System;

namespace Notes.Net.Common
{
    public class DateHelper
    {
        public static string PrettyTimeSpan(TimeSpan ts)
        {
            if (ts.TotalMilliseconds < 1000)
                return $"{(int)ts.TotalMilliseconds} ms ago";

            if (ts.TotalSeconds < 60)
                return $"{(int)ts.TotalSeconds} sec. ago";

            if (ts.TotalMinutes < 60)
                return $"{(int)ts.TotalMinutes} min. ago";

            if (ts.TotalHours < 60)
                return $"{(int)ts.TotalHours} hours ago";

            return $"{(int)ts.TotalDays} days ago";
        }
    }
}
