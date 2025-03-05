namespace Plant_Explorer.Contract.Repositories.ModelViews.UserPointModel
{
    public class PutUserPointModel
    {
        public string UserId { get; set; } = string.Empty;

        public int AdditionalPoint { get; set; }
    }
}
