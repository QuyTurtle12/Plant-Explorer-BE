using System;

namespace Plant_Explorer.Core.Utils
{
    public class DateTimeParsing
    {
        public static DateTime ConvertToTimeZone(DateTime dateTime)
        {
            // Get the target time zone information
            TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert the given DateTime to the target time zone
            return TimeZoneInfo.ConvertTime(dateTime, targetTimeZone);
        }

        public static DateTime ConvertToUtcPlus7(DateTime dateTime)
        {
            // UTC+7 is 7 hours ahead of UTC
            return dateTime.AddHours(7);
        }

        public static DateTime ConvertToUtcPlus7NotChanges(DateTime dateTime)
        {
            // Keep the original date and time but assume UTC+7
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified).AddHours(7);
        }
    }
}
