using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizModel;
using Plant_Explorer.Contract.Repositories.ModelViews.UserPointModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Services.Services
{
    public class QuizService : IQuizService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPointService _userPointService;
        private readonly IQuizAttemptService _quizAttemptService;

        public const int INACTIVE = 0;
        public const int ACTIVE = 1;

        public QuizService(IMapper mapper, IUnitOfWork unitOfWork, IUserPointService userPointService, IQuizAttemptService quizAttemptService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userPointService = userPointService;
            _quizAttemptService = quizAttemptService;
        }

        public async Task CreateQuizAsync(PostQuizModel newQuiz)
        {
            GeneralValidation(newQuiz);

            // Mapping model to entities
            Quiz quiz = _mapper.Map<Quiz>(newQuiz);

            // Add new quiz to database and save
            await _unitOfWork.GetRepository<Quiz>().InsertAsync(quiz);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteQuizAsync(string id)
        {
            // Validate if quiz existed
            Quiz? existingQuiz = await _unitOfWork.GetRepository<Quiz>().Entities
                                                        .Where(q => q.Id.Equals(Guid.Parse(id)))
                                                        .FirstOrDefaultAsync()
                                                        ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This quiz is not exist!");

            // Delete item
            existingQuiz.Status = INACTIVE;

            // Update audit fields
            existingQuiz.LastUpdatedTime = CoreHelper.SystemTimeNow;
            existingQuiz.DeletedTime = existingQuiz.LastUpdatedTime;

            // Save
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetQuizModel>> GetAllQuizzesAsync(int index, int pageSize, string? idSearch, string? nameSearch)
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

            // Get list of quizzes
            IQueryable<Quiz> query = _unitOfWork.GetRepository<Quiz>().Entities;

            // Check if user want to search a quiz by quizId
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                // Convert to guid
                Guid.TryParse(idSearch, out Guid id);

                // Search quiz by id
                query = query.Where(q => q.Id.Equals(id));
            }

            // Check if user want to search quizzes by name 
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                // Search quiz by name
                query = query.Where(q => q.Name.Contains(nameSearch));
            }

            // Skip deleted item
            query = query.Where(q => !q.DeletedTime.HasValue);

            // Sort the list by name
            query = query.OrderBy(q => q.Name);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<Quiz> resultQuery = await _unitOfWork.GetRepository<Quiz>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetQuizModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map Quiz to ModelView in order to filter unnecessary data 
                GetQuizModel quizModel = _mapper.Map<GetQuizModel>(item);

                // Format audit fields
                quizModel.CreatedTime = item.CreatedTime?.ToString("dd-MM-yyyy");
                quizModel.LastUpdatedTime = item.LastUpdatedTime?.ToString("dd-MM-yyyy");

                return quizModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetQuizModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        public async Task<GetQuizModel> GetQuizByIdAsync(string id)
        {
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(Guid.Parse(id));

            // Validate if quiz is existed
            if (quiz == null || quiz.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Quiz not found!");

            GetQuizModel quizModel = _mapper.Map<GetQuizModel>(quiz);

            // Format audit fields
            quizModel.CreatedTime = quiz.CreatedTime?.ToString("dd-MM-yyyy");
            quizModel.LastUpdatedTime = quiz.LastUpdatedTime?.ToString("dd-MM-yyyy");

            return quizModel;
        }

        public async Task UpdateQuizAsync(string id, PutQuizModel updatedQuiz)
        {
            // Validate if quiz existed
            Quiz? existingQuiz = await _unitOfWork.GetRepository<Quiz>().Entities
                                                        .Where(q => q.Id.Equals(Guid.Parse(id)))
                                                        .FirstOrDefaultAsync()
                                                        ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This quiz is not exist!");

            // Validate input
            GeneralValidation(updatedQuiz);

            // Mapping model to entities
            _mapper.Map(updatedQuiz, existingQuiz);

            // Update audit field
            existingQuiz.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Update quiz info and save
            _unitOfWork.GetRepository<Quiz>().Update(existingQuiz);
            await _unitOfWork.SaveAsync();
        }

        private void GeneralValidation(BaseQuizModel quiz)
        {
            // Validate quiz's name
            if (string.IsNullOrWhiteSpace(quiz.Name)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");
        }

        public async Task<int> AnswerQuizAsync(string quizId, IList<AnswerQuestionModel> answerList)
        {
            // Validate if answer is empty
            if(answerList == null || answerList.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "answer must not be empty!");
            }

            // Number of correct answer that user inputted
            int point = 0;

            // Check answer
            foreach (AnswerQuestionModel item in answerList)
            {
                // Validate if the question is belong to given quiz
                Question? question = await _unitOfWork.GetRepository<Question>().Entities
                                                    .Where(q => q.QuizId.Equals(Guid.Parse(quizId)) && q.Id.Equals(Guid.Parse(item.QuestionId)))
                                                    .FirstOrDefaultAsync();

                if (question == null) 
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, $"There is a mixed question of another quiz in the answer, question id: {item.QuestionId}");
                }

                // Validate if option is belong to given question and is correct answer
                Option? option = await _unitOfWork.GetRepository<Option>().Entities
                                                .Where(o => o.Id.Equals(Guid.Parse(item.OptionId)) && o.QuestionId.Equals(Guid.Parse(item.QuestionId)) && o.IsCorrect == true)
                                                .FirstOrDefaultAsync();

                // Increase 1 point if answer is correct 
                if (option != null)
                {
                    point += (int)question.Point!;
                }
            }

            // Initialize Quiz Attempt Model
            PostQuizAttemptModel attempt = new PostQuizAttemptModel()
            {
                Score = 0,
                QuizId = Guid.Parse(quizId),
                AttemptTime = CoreHelper.SystemTimeNow
            };

            // Create a new user attempt
            await _quizAttemptService.CreateQuizAttemptAsync(attempt);

            // Initialize point model
            PutUserPointModel additionalPoint = new PutUserPointModel()
            {
                AdditionalPoint = point
            };

            // Add new point to current logged in child
            await _userPointService.UpdateUserPointAsync(additionalPoint);

            return point;
        }
    }
}