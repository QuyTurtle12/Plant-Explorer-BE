using Plant_Explorer.Core.Constants.Enum.EnumUser;

namespace Plant_Explorer.Contract.Repositories.ModelViews.UserModel
{
    public class BaseUserModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
