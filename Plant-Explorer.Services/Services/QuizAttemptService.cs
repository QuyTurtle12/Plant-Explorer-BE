using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Services.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public QuizAttemptService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateQuizAttemptAsync(PostQuizAttemptModel newAttempt)
    {
        // Validate if quiz exists
        if (!string.IsNullOrWhiteSpace(newAttempt.QuizId.ToString()))
        {
            Guid quizId = newAttempt.QuizId;
            bool quizExists = await _unitOfWork.GetRepository<Quiz>().Entities
                                    .AnyAsync(q => q.Id == quizId && !q.DeletedTime.HasValue);

            if (!quizExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Specified quiz does not exist!");
        }
        else
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Quiz ID must not be empty!");
        }

        // Validate user ID
        if (string.IsNullOrWhiteSpace(newAttempt.UserId.ToString()))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "User ID must not be empty!");
        }

        // Mapping model to entities
        QuizAttempt quizAttempt = _mapper.Map<QuizAttempt>(newAttempt);

        // Add new quiz attempt to database and save
        await _unitOfWork.GetRepository<QuizAttempt>().InsertAsync(quizAttempt);
        await _unitOfWork.SaveAsync();
    }

    public async Task<PaginatedList<GetQuizAttemptModel>> GetAllQuizAttemptsAsync(int index, int pageSize, string? userId, string? quizId)
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

        // Get list of quiz attempts
        IQueryable<QuizAttempt> query = _unitOfWork.GetRepository<QuizAttempt>().Entities;

        // Check if user want to filter by userId
        if (!string.IsNullOrWhiteSpace(userId))
        {
            // Filter by user id
            query = query.Where(qa => qa.UserId.Equals(userId));
        }

        // Check if user want to filter by quizId
        if (!string.IsNullOrWhiteSpace(quizId))
        {
            // Convert to guid
            Guid.TryParse(quizId, out Guid id);

            // Filter by quiz id
            query = query.Where(qa => qa.QuizId.Equals( id.ToString()));
        }

        // Skip deleted item
        query = query.Where(qa => !qa.DeletedTime.HasValue);

        // Sort the list by attempt time (most recent first)
        query = query.OrderByDescending(qa => qa.AttemptTime);

        // Change to paginated list type to facilitate filtering process
        PaginatedList<QuizAttempt> resultQuery = await _unitOfWork.GetRepository<QuizAttempt>().GetPagging(query, index, pageSize);

        // Filter unnecessary data
        IReadOnlyCollection<GetQuizAttemptModel> responseItems = resultQuery.Items.Select(item =>
        {
            // Map QuizAttempt to ModelView in order to filter unnecessary data 
            GetQuizAttemptModel quizAttemptModel = _mapper.Map<GetQuizAttemptModel>(item);

            // Format audit fields
            quizAttemptModel.CreatedTime = item.CreatedTime?.ToString("dd-MM-yyyy");
            quizAttemptModel.LastUpdatedTime = item.LastUpdatedTime?.ToString("dd-MM-yyyy");

            return quizAttemptModel;
        }).ToList();

        // Create a new paginated list for response data
        PaginatedList<GetQuizAttemptModel> paginatedList = new(
            responseItems,
            resultQuery.TotalCount,
            resultQuery.PageNumber,
            resultQuery.TotalPages
            );

        return paginatedList;
    }

    public async Task<GetQuizAttemptModel> GetQuizAttemptByIdAsync(string id)
    {
        QuizAttempt? quizAttempt = await _unitOfWork.GetRepository<QuizAttempt>().GetByIdAsync(Guid.Parse(id));

        // Validate if quiz attempt is existed
        if (quizAttempt == null || quizAttempt.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Quiz attempt not found!");

        GetQuizAttemptModel quizAttemptModel = _mapper.Map<GetQuizAttemptModel>(quizAttempt);

        // Format audit fields
        quizAttemptModel.CreatedTime = quizAttempt.CreatedTime?.ToString("dd-MM-yyyy");
        quizAttemptModel.LastUpdatedTime = quizAttempt.LastUpdatedTime?.ToString("dd-MM-yyyy");

        return quizAttemptModel;
    }
}