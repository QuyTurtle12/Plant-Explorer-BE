using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Question management controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="questionService">Question service</param>
        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        /// <summary>
        /// Get all questions
        /// </summary>
        /// <param name="index">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="idSearch">Question id</param>
        /// <param name="nameSearch">Question name</param>
        /// <param name="quizId">Quiz id</param>
        /// <returns>List of questions</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllQuestions(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, string? quizId = null)
        {
            PaginatedList<GetQuestionModel> result = await _questionService.GetAllQuestionsAsync(index, pageSize, idSearch, nameSearch, quizId);
            return Ok(new BaseResponseModel<PaginatedList<GetQuestionModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Get question by id
        /// </summary>
        /// <param name="id">Question id</param>
        /// <returns>Question details</returns>
        [HttpGet]
        [Route("question")]
        public async Task<IActionResult> GetQuestion(string id)
        {
            GetQuestionModel result = await _questionService.GetQuestionByIdAsync(id);
            return Ok(new BaseResponseModel<GetQuestionModel>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="newQuestion">Question details</param>
        /// <returns>Creation result</returns>
        [HttpPost]
        [Route("question")]
        public async Task<IActionResult> CreateQuestion(PostQuestionModel newQuestion)
        {
            await _questionService.CreateQuestionAsync(newQuestion);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Create a new question successfully"
                ));
        }

        /// <summary>
        /// Update a question
        /// </summary>
        /// <param name="id">Question id</param>
        /// <param name="updatedQuestion">Updated question details</param>
        /// <returns>Update result</returns>
        [HttpPut]
        [Route("question")]
        public async Task<IActionResult> UpdateQuestion(string id, PutQuestionModel updatedQuestion)
        {
            await _questionService.UpdateQuestionAsync(id, updatedQuestion);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Update a question successfully"
                ));
        }

        /// <summary>
        /// Delete a question
        /// </summary>
        /// <param name="id">Question id</param>
        /// <returns>Deletion result</returns>
        [HttpDelete]
        [Route("question")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            await _questionService.DeleteQuestionAsync(id);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Delete a question successfully"
                ));
        }
    }
}