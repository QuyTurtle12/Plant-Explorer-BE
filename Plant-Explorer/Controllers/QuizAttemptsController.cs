using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Services.Services;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Quiz Attempt management controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuizAttemptsController : ControllerBase
    {
        private readonly IQuizAttemptService _quizAttemptService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="quizAttemptService">Quiz Attempt service</param>
        public QuizAttemptsController(IQuizAttemptService quizAttemptService)
        {
            _quizAttemptService = quizAttemptService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllQuizAttempts(int index = 1, int pageSize = 10, string? userId = null, string? quizId = null)
        //{
        //    PaginatedList<GetQuizAttemptModel> result = await _quizAttemptService.GetAllQuizAttemptsAsync(index, pageSize, userId, quizId);
        //    return Ok(new BaseResponseModel<PaginatedList<GetQuizAttemptModel>>(
        //        statusCode: StatusCodes.Status200OK,
        //        code: ResponseCodeConstants.SUCCESS,
        //        data: result,
        //        additionalData: null,
        //        message: "Finished"
        //    ));
        //}

        //[HttpGet]
        //[Route("quizattempt")]
        //public async Task<IActionResult> GetQuizAttempt(string id)
        //{
        //    GetQuizAttemptModel result = await _quizAttemptService.GetQuizAttemptByIdAsync(id);
        //    return Ok(new BaseResponseModel<GetQuizAttemptModel>(
        //        statusCode: StatusCodes.Status200OK,
        //        code: ResponseCodeConstants.SUCCESS,
        //        data: result,
        //        additionalData: null,
        //        message: "Finished"
        //    ));
        //}

        //[HttpPost]
        //[Route("quizattempt")]
        //public async Task<IActionResult> CreateQuizAttempt(PostQuizAttemptModel newQuizAttempt)
        //{
        //    await _quizAttemptService.CreateQuizAttemptAsync(newQuizAttempt);
        //    return Ok(new BaseResponseModel(
        //        statusCode: StatusCodes.Status201Created,
        //        code: ResponseCodeConstants.SUCCESS,
        //        message: "Create a new quiz attempt successfully"
        //    ));
        //}
    }
}