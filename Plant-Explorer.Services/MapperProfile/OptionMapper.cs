using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;

namespace Plant_Explorer.Mapping
{
    public static class OptionMapper
    {
        public static GetOptionModel ToGetOptionModel(Option option)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));

            return new GetOptionModel
            {
                // Assume BaseEntity has Id as Guid
                Id = option.Id, // Convert Guid to string

                // BaseOptionModel properties
                QuestionId = option.QuestionId.ToString(), // Convert Guid to string
                Context = option.Context ?? string.Empty, // Handle null to match string.Empty default
                IsCorrect = option.IsCorrect,

                // GetOptionModel properties
                CreatedTime = option.CreatedTime?.ToString("o") ?? DateTime.UtcNow.ToString("o"), // ISO 8601 format
                LastUpdatedTime = option.LastUpdatedTime?.ToString("o") ?? DateTime.UtcNow.ToString("o")
            };
        }
    }
}