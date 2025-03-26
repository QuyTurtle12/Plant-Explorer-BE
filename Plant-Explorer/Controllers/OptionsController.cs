using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;

using Plant_Explorer.Core.Constants;

/// <summary>
/// Option management controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OptionsController : ControllerBase
{
    private readonly IOptionService _optionService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="optionService">Option service</param>
    public OptionsController(IOptionService optionService)
    {
        _optionService = optionService;
    }

    /// <summary>
    /// Get all options
    /// </summary>
    /// <param name="index">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="idSearch">Option id</param>
    /// <param name="nameSearch">Option name</param>
    /// <param name="questionId">Question id</param>
    /// <returns>List of options</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOptions(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, string? questionId = null)
    {
        PaginatedList<GetOptionModel> result = await _optionService.GetAllOptionsAsync(index, pageSize, idSearch, nameSearch, questionId);
        return Ok(new BaseResponseModel<PaginatedList<GetOptionModel>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: result,
            additionalData: null,
            message: "Finished"
            ));
    }

    /// <summary>
    /// Get option by id
    /// </summary>
    /// <param name="id">Option id</param>
    /// <returns>Option details</returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetOption(string id)
    {
        GetOptionModel result = await _optionService.GetOptionByIdAsync(id);
        return Ok(new BaseResponseModel<GetOptionModel>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: result,
            additionalData: null,
            message: "Finished"
            ));
    }

    /// <summary>
    /// Create a new option
    /// </summary>
    /// <param name="newOption">Option details</param>
    /// <returns>Creation result</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOption(PostOptionModel newOption)
    {
        await _optionService.CreateOptionAsync(newOption);
        return Ok(new BaseResponseModel(
            statusCode: StatusCodes.Status201Created,
            code: ResponseCodeConstants.SUCCESS,
            message: "Create a new option successfully"
            ));
    }

    /// <summary>
    /// Update an option
    /// </summary>
    /// <param name="id">Option id</param>
    /// <param name="updatedOption">Updated option details</param>
    /// <returns>Update result</returns>
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateOption(string id, PutOptionModel updatedOption)
    {
        await _optionService.UpdateOptionAsync(id, updatedOption);
        return Ok(new BaseResponseModel(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update an option successfully"
            ));
    }

    /// <summary>
    /// Delete an option
    /// </summary>
    /// <param name="id">Option id</param>
    /// <returns>Deletion result</returns>
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteOption(string id)
    {
        await _optionService.DeleteOptionAsync(id);
        return Ok(new BaseResponseModel(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Delete an option successfully"
            ));
    }

    /// <summary>
    /// Check if an option is correct for a specific question
    /// </summary>
    /// <param name="optionId">Option ID</param>
    /// <param name="questionId">Question ID</param>
    /// <returns>True if the option is correct, false otherwise</returns>
    [HttpGet]
    [Route("check-correct/{id}")]
    public async Task<IActionResult> CheckCorrectOption(string id)
    {
        bool isCorrect = await _optionService.IsCorrectOptionAsync(id);
        return Ok(new BaseResponseModel<bool>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: isCorrect,
            additionalData: null,
            message: "Option correctness checked successfully"
        ));
    }

}