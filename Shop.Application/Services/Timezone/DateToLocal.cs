using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Services.Timezone
{
    public static class DateToLocal
    {
        private static readonly string zaTimeZoneIdentifier = "South Africa Standard Time";

        public static DateTime ConvertTimeToLocal(this DateTime dateTime)
        {
            TimeZoneInfo zaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(zaTimeZoneIdentifier);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, zaTimeZone);
        }

        public static DateTime? ConvertTimeToLocal(this DateTime? dateTime)
        {
            if (dateTime == null)
                return null;

            TimeZoneInfo zaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(zaTimeZoneIdentifier);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.Value, zaTimeZone);
        }
    }
}
