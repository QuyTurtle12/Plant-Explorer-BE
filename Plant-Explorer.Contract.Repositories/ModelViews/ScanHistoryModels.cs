namespace Plant_Explorer.Contract.Repositories.ModelViews
{
    public class ScanHistoryBaseModel
    {
        public Guid UserId { get; set; }
        public Guid PlantId { get; set; }
        public decimal? Probability { get; set; }
        public string? ImgUrl { get; set; }
    }
    public class ScanHistoryGetModel : ScanHistoryBaseModel
    {
        public Guid Id { get; set; }
        public DateTime ScanTime { get; set; }
        
    }

    public class ScanHistoryPostModel : ScanHistoryBaseModel
    {
    }

}
