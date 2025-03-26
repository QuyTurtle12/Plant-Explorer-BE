namespace Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel
{
    public class BaseBadgeModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int? conditionalPoint { get; set; }
        public string? imageUrl { get; set; }
    }
}
