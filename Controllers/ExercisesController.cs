using GymManagementApi.DTOs;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll(
        [FromQuery] string? muscleGroup,
        [FromQuery] string? search)
    {
        var exercises = await _exerciseService.GetAllAsync(muscleGroup, search);
        return Ok(exercises);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ExerciseDto>> GetById(int id)
    {
        var result = await _exerciseService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Exercise not found."));
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ExerciseDto>> Create([FromBody] CreateExerciseDto dto)
    {
        var result = await _exerciseService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to create exercise.", result.ValidationErrors));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ExerciseDto>> Update(int id, [FromBody] UpdateExerciseDto dto)
    {
        var result = await _exerciseService.UpdateAsync(id, dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to update exercise.", result.ValidationErrors));
        }

        return Ok(result.Data);
    }

    [HttpPut("{id:int}/media")]
    [AllowAnonymous]
    public async Task<ActionResult<ExerciseDto>> UpdateMedia(int id, [FromBody] UpdateExerciseMediaDto dto)
    {
        var result = await _exerciseService.UpdateMediaAsync(id, dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to update exercise media."));
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _exerciseService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to delete exercise."));
        }

        return Ok(new { statusCode = 200, message = result.ErrorMessage ?? "Exercise deleted successfully." });
    }
}
