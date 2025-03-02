namespace Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel
{
    public class GetUserBadgeModel : BaseUserBadgeModel
    {
        private string Id { get; set; } = string.Empty;
        public string DateEarned { get; set; } = string.Empty;
    }
}
