using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public const int INACTIVE = 0;
        public const int ACTIVE = 1;

        public QuestionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateQuestionAsync(PostQuestionModel newQuestion)
        {
            GeneralValidation(newQuestion);

            // Validate if quiz exists
            if (!string.IsNullOrWhiteSpace(newQuestion.QuizId))
            {
                Guid quizId = Guid.Parse(newQuestion.QuizId);
                bool quizExists = await _unitOfWork.GetRepository<Quiz>().Entities
                                        .AnyAsync(q => q.Id == quizId && !q.DeletedTime.HasValue);

                if (!quizExists)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Specified quiz does not exist!");
            }

            // Mapping model to entities
            Question question = _mapper.Map<Question>(newQuestion);

            // Add new question to database and save
            await _unitOfWork.GetRepository<Question>().InsertAsync(question);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteQuestionAsync(string id)
        {
            // Validate if question existed
            Question? existingQuestion = await _unitOfWork.GetRepository<Question>().Entities
                                                        .Where(q => q.Id.Equals(Guid.Parse(id)))
                                                        .FirstOrDefaultAsync()
                                                        ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This question is not exist!");

            // Delete item
            existingQuestion.Status = INACTIVE;

            // Update audit fields
            existingQuestion.LastUpdatedTime = CoreHelper.SystemTimeNow;
            existingQuestion.DeletedTime = existingQuestion.LastUpdatedTime;

            // Save
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetQuestionModel>> GetAllQuestionsAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? quizId)
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

            // Get list of questions
            IQueryable<Question> query = _unitOfWork.GetRepository<Question>().Entities;

            // Check if user want to search a question by questionId
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                // Convert to guid
                Guid.TryParse(idSearch, out Guid id);

                // Search question by id
                query = query.Where(q => q.Id.Equals(id));
            }

            // Check if user want to search questions by name 
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                // Search question by name
                query = query.Where(q => q.Name.Contains(nameSearch));
            }

            // Check if user want to filter by quizId
            if (!string.IsNullOrWhiteSpace(quizId))
            {
                // Convert to guid
                Guid.TryParse(quizId, out Guid id);

                // Filter by quiz id
                query = query.Where(q => q.QuizId.Equals(id.ToString()));
            }

            // Skip deleted item
            query = query.Where(q => !q.DeletedTime.HasValue);

            // Sort the list by name
            query = query.OrderBy(q => q.Name);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<Question> resultQuery = await _unitOfWork.GetRepository<Question>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetQuestionModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map Question to ModelView in order to filter unnecessary data 
                GetQuestionModel questionModel = _mapper.Map<GetQuestionModel>(item);

                // Format audit fields
                questionModel.CreatedTime = item.CreatedTime?.ToString("dd-MM-yyyy");
                questionModel.LastUpdatedTime = item.LastUpdatedTime?.ToString("dd-MM-yyyy");

                return questionModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetQuestionModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        public async Task<GetQuestionModel> GetQuestionByIdAsync(string id)
        {
            Question? question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(Guid.Parse(id));

            // Validate if question is existed
            if (question == null || question.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Question not found!");

            GetQuestionModel questionModel = _mapper.Map<GetQuestionModel>(question);

            // Format audit fields
            questionModel.CreatedTime = question.CreatedTime?.ToString("dd-MM-yyyy");
            questionModel.LastUpdatedTime = question.LastUpdatedTime?.ToString("dd-MM-yyyy");

            return questionModel;
        }

        public async Task UpdateQuestionAsync(string id, PutQuestionModel updatedQuestion)
        {
            // Validate if question existed
            Question? existingQuestion = await _unitOfWork.GetRepository<Question>().Entities
                                                        .Where(q => q.Id.Equals(Guid.Parse(id)))
                                                        .FirstOrDefaultAsync()
                                                        ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This question is not exist!");

            // Validate input
            GeneralValidation(updatedQuestion);

            // Validate if quiz exists
            if (!string.IsNullOrWhiteSpace(updatedQuestion.QuizId) && !updatedQuestion.QuizId.Equals( existingQuestion.QuizId))
            {
                Guid quizId = Guid.Parse(updatedQuestion.QuizId);
                bool quizExists = await _unitOfWork.GetRepository<Quiz>().Entities
                                        .AnyAsync(q => q.Id == quizId && !q.DeletedTime.HasValue);

                if (!quizExists)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Specified quiz does not exist!");
            }

            // Mapping model to entities
            _mapper.Map(updatedQuestion, existingQuestion);

            // Update audit field
            existingQuestion.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Update question info and save
            _unitOfWork.GetRepository<Question>().Update(existingQuestion);
            await _unitOfWork.SaveAsync();
        }

        private void GeneralValidation(BaseQuestionModel question)
        {
            // Validate question's name
            if (string.IsNullOrWhiteSpace(question.Name)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");
        }
    }
}