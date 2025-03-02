using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Services.Services;

namespace Plant_Explorer.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes(int index = 1, int pageSize = 10, string? nameSearch = null)
        {
            PaginatedList<Quiz> result = await _quizService.GetAllQuizzesAsync(index, pageSize, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<Quiz>>(
                StatusCodes.Status200OK,
                "SUCCESS",
                result,
                null,
                "Finished"
            ));
        }

        [HttpGet("quiz")]
        public async Task<IActionResult> GetQuiz(string id)
        {
            Quiz result = await _quizService.GetQuizByIdAsync(id);
            return Ok(new BaseResponseModel<Quiz>(
                StatusCodes.Status200OK,
                "SUCCESS",
                result,
                null,
                "Finished"
            ));
        }

        [HttpPost("quiz")]
        public async Task<IActionResult> CreateQuiz(CreateQuizDto newQuiz)
        {
            await _quizService.CreateQuizAsync(newQuiz);
            return Ok(new BaseResponseModel(
                StatusCodes.Status201Created,
                "SUCCESS",
                "Create a new quiz successfully"
            ));
        }

        [HttpPut("quiz")]
        public async Task<IActionResult> UpdateQuiz(string id, UpdateQuizDto updatedQuiz)
        {
            await _quizService.UpdateQuizAsync(id, updatedQuiz);
            return Ok(new BaseResponseModel(
                StatusCodes.Status200OK,
                "SUCCESS",
                "Update a quiz successfully"
            ));
        }

        [HttpDelete("quiz")]
        public async Task<IActionResult> DeleteQuiz(string id)
        {
            await _quizService.DeleteQuizAsync(id);
            return Ok(new BaseResponseModel(
                StatusCodes.Status200OK,
                "SUCCESS",
                "Delete a quiz successfully"
            ));
        }

    } 
}
