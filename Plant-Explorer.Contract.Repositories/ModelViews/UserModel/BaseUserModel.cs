using Plant_Explorer.Core.Constants.Enum.EnumUser;

namespace Plant_Explorer.Contract.Repositories.ModelViews.UserModel
{
    public class BaseUserModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Role { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
