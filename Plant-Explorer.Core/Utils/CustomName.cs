using Microsoft.AspNetCore.Http;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;


namespace Plant_Explorer.Core.Utils
{
    public static class CustomName
    {
        public static string GetCustomName<T>(this T enumValue) where T : struct, Enum
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.FAILED, $"Invalid enum value {enumValue} for enum type {typeof(T)}");
            }

            var attribute = fieldInfo
                .GetCustomAttributes(typeof(CustomNameAttribute), false)
                .FirstOrDefault() as CustomNameAttribute;

            //var attribute = enumValue.GetType().GetField(enumValue.ToString())
            //    .GetCustomAttributes(typeof(CustomNameAttribute), false)
            //    .FirstOrDefault() as CustomNameAttribute;

            return attribute?.Name ?? enumValue.ToString();
        }
    }
}
