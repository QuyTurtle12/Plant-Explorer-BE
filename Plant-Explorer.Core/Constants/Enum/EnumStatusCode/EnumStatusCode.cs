using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Core.Constants.Enum.EnumStatusCode
{
    public enum EnumStatusCode
    {
        [CustomName("Success")]
        OK = 200,

        [CustomName("Bad Request")]
        BadRequest = 400,

        [CustomName("Unauthorized")]
        Unauthorized = 401,

        [CustomName("Internal Server Error")]
        ServerError = 500
    }
}
