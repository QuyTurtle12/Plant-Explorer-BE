namespace Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel
{
    public class GetBadgeModel : BaseBadgeModel
    {
        public string Id { get; set; } = string.Empty;
        public string CreatedTime { get; set; } = string.Empty;
        public string LastUpdatedTime { get; set; } = string.Empty;
    }
}
