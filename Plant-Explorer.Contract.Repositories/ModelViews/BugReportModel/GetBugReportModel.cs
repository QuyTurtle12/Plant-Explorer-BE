namespace Plant_Explorer.Contract.Repositories.ModelViews.BugReportModel
{
    public class GetBugReportModel : BaseBugReportModel
    {
        public string Id { get; set; } = string.Empty;
        public string CreatedTime { get; set; } = string.Empty;
        public string CreatedBy { get; set;} = string.Empty;
    }
}
