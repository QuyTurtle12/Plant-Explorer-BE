namespace Plant_Explorer.Contract.Repositories.ModelViews.UserModel
{
    public class PostUserModel : BaseUserModel
    {
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
