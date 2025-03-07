using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Quiz management controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="quizService">Quiz service</param>
        public QuizzesController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        /// <summary>
        /// Get all quizzes
        /// </summary>
        /// <param name="index">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="idSearch">Quiz id</param>
        /// <param name="nameSearch">Quiz name</param>
        /// <returns>List of quizzes</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null)
        {
            PaginatedList<GetQuizModel> result = await _quizService.GetAllQuizzesAsync(index, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetQuizModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Get quiz by id
        /// </summary>
        /// <param name="id">Quiz id</param>
        /// <returns>Quiz details</returns>
        [HttpGet]
        [Route("quiz")]
        public async Task<IActionResult> GetQuiz(string id)
        {
            GetQuizModel result = await _quizService.GetQuizByIdAsync(id);
            return Ok(new BaseResponseModel<GetQuizModel>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Create a new quiz
        /// </summary>
        /// <param name="newQuiz">Quiz details</param>
        /// <returns>Creation result</returns>
        [HttpPost]
        [Route("quiz")]
        public async Task<IActionResult> CreateQuiz(PostQuizModel newQuiz)
        {
            await _quizService.CreateQuizAsync(newQuiz);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Create a new quiz successfully"
                ));
        }

        /// <summary>
        /// Update a quiz
        /// </summary>
        /// <param name="id">Quiz id</param>
        /// <param name="updatedQuiz">Updated quiz details</param>
        /// <returns>Update result</returns>
        [HttpPut]
        [Route("quiz")]
        public async Task<IActionResult> UpdateQuiz(string id, PutQuizModel updatedQuiz)
        {
            await _quizService.UpdateQuizAsync(id, updatedQuiz);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Update a quiz successfully"
                ));
        }

        /// <summary>
        /// Delete a quiz
        /// </summary>
        /// <param name="id">Quiz id</param>
        /// <returns>Deletion result</returns>
        [HttpDelete]
        [Route("quiz")]
        public async Task<IActionResult> DeleteQuiz(string id)
        {
            await _quizService.DeleteQuizAsync(id);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Delete a quiz successfully"
                ));
        }
    }
}

    // Similar controllers for QuestionsController, OptionsController, and QuizAttemptsController would follow the same pattern