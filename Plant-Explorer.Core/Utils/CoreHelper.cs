namespace Plant_Explorer.Core.Utils
{
    public class CoreHelper
    {
        public static DateTime SystemTimeNow => DateTimeParsing.ConvertToUtcPlus7(DateTime.Now);
    }
}
