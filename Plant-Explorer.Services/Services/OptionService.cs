using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Core.Utils;

public class OptionService : IOptionService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public const int INACTIVE = 0;
    public const int ACTIVE = 1;

    public OptionService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateOptionAsync(PostOptionModel newOption)
    {
        GeneralValidation(newOption);
        var questionId = newOption.QuestionId;

        // Validate if question exists
        if (!string.IsNullOrWhiteSpace(newOption.QuestionId.ToString()))
        {
           
            bool questionExists = await _unitOfWork.GetRepository<Question>().Entities
                                        .AnyAsync(q => q.Id == questionId && !q.DeletedTime.HasValue);

            if (!questionExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Specified question does not exist!");
        }

        // Mapping model to entities
        Option option = _mapper.Map<Option>(newOption);

        // Add new option to database and save
        await _unitOfWork.GetRepository<Option>().InsertAsync(option);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteOptionAsync(string id)
    {
        // Validate if option existed
        Option? existingOption = await _unitOfWork.GetRepository<Option>().Entities
                                                .Where(o => o.Id.Equals(Guid.Parse(id)))
                                                .FirstOrDefaultAsync()
                                                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This option is not exist!");

        // Delete item
        existingOption.Status = INACTIVE;

        // Update audit fields
        existingOption.LastUpdatedTime = CoreHelper.SystemTimeNow;
        existingOption.DeletedTime = existingOption.LastUpdatedTime;

        // Save
        await _unitOfWork.SaveAsync();
    }

    public async Task<PaginatedList<GetOptionModel>> GetAllOptionsAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? questionId)
    {
        // index checking
        if (index <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "index need to be bigger than 0");
        }

        // pageSize checking
        if (pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "pageSize need to be bigger than 0");
        }

        // Get list of options
        IQueryable<Option> query = _unitOfWork.GetRepository<Option>().Entities;

        // Check if user want to search an option by optionId
        if (!string.IsNullOrWhiteSpace(idSearch))
        {
            // Convert to guid
            Guid.TryParse(idSearch, out Guid id);

            // Search option by id
            query = query.Where(o => o.Id.Equals(id));
        }

        // Check if user want to search options by name 
        if (!string.IsNullOrWhiteSpace(nameSearch))
        {
            // Search option by name
            query = query.Where(o => o.Name.Contains(nameSearch));
        }

        // Check if user want to filter by questionId
        if (!string.IsNullOrWhiteSpace(questionId))
        {
            // Convert to guid
            Guid.TryParse(questionId, out Guid id);

            // Filter by question id
            query = query.Where(o => o.QuestionId.Equals(id.ToString()));
        }

        // Skip deleted item
        query = query.Where(o => !o.DeletedTime.HasValue);

        // Sort the list by name
        query = query.OrderBy(o => o.Name);

        // Change to paginated list type to facilitate filtering process
        PaginatedList<Option> resultQuery = await _unitOfWork.GetRepository<Option>().GetPagging(query, index, pageSize);

        // Filter unnecessary data
        IReadOnlyCollection<GetOptionModel> responseItems = resultQuery.Items.Select(item =>
        {
            // Map Option to ModelView in order to filter unnecessary data 
            GetOptionModel optionModel = Plant_Explorer.Mapping.OptionMapper.ToGetOptionModel(item);

            // Format audit fields
            optionModel.CreatedTime = item.CreatedTime?.ToString("dd-MM-yyyy");
            optionModel.LastUpdatedTime = item.LastUpdatedTime?.ToString("dd-MM-yyyy");

            return optionModel;
        }).ToList();

        // Create a new paginated list for response data
        PaginatedList<GetOptionModel> paginatedList = new(
            responseItems,
            resultQuery.TotalCount,
            resultQuery.PageNumber,
            resultQuery.TotalPages
            );

        return paginatedList;
    }

    public async Task<GetOptionModel> GetOptionByIdAsync(string id)
    {
        Option? option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(Guid.Parse(id));

        // Validate if option is existed
        if (option == null || option.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Option not found!");

        GetOptionModel optionModel = Plant_Explorer.Mapping.OptionMapper.ToGetOptionModel(option);

        // Format audit fields
        optionModel.CreatedTime = option.CreatedTime?.ToString("dd-MM-yyyy");
        optionModel.LastUpdatedTime = option.LastUpdatedTime?.ToString("dd-MM-yyyy");

        return optionModel;
    }

    public async Task UpdateOptionAsync(string id, PutOptionModel updatedOption)
    {
        // Validate if option existed
        Option? existingOption = await _unitOfWork.GetRepository<Option>().Entities
                                                .Where(o => o.Id.Equals(Guid.Parse(id)))
                                                .FirstOrDefaultAsync()
                                                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This option is not exist!");

        // Validate input
        GeneralValidation(updatedOption);

        // Validate if question exists
        if (!string.IsNullOrWhiteSpace(updatedOption.QuestionId.ToString()) && updatedOption.QuestionId.Equals(existingOption.QuestionId))
        {
            Guid questionId = Guid.Parse(updatedOption.QuestionId.ToString());
            bool questionExists = await _unitOfWork.GetRepository<Question>().Entities
                                        .AnyAsync(q => q.Id == questionId && !q.DeletedTime.HasValue);

            if (!questionExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Specified question does not exist!");
        }

        // Mapping model to entities
        _mapper.Map(updatedOption, existingOption);

        // Update audit field
        existingOption.LastUpdatedTime = CoreHelper.SystemTimeNow;

        // Update option info and save
        _unitOfWork.GetRepository<Option>().Update(existingOption);
        await _unitOfWork.SaveAsync();
    }

    private void GeneralValidation(BaseOptionModel option)
    {
        // Validate option's name
       // if (string.IsNullOrWhiteSpace(option.Name)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");
    }
}