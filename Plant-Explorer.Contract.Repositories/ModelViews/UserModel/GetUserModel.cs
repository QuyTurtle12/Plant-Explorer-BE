namespace Plant_Explorer.Contract.Repositories.ModelViews.UserModel
{
    public class GetUserModel : BaseUserModel
    {
        public string Id { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Status { get; set; }
        public string CreatedTime { get; set; } = string.Empty; 
        public string LastUpdatedTime { get; set; } = string.Empty; 
    }
}
