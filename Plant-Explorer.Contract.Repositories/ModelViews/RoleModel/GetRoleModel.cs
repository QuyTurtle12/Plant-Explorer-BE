namespace Plant_Explorer.Contract.Repositories.ModelViews.RoleModel
{
    public class GetRoleModel : BaseRoleModel
    {
        public string Id { get; set; } = string.Empty;
        public string CreatedTime { get; set; } = string.Empty;
        public string LastUpdatedTime { get; set; } = string.Empty;
    }
}
