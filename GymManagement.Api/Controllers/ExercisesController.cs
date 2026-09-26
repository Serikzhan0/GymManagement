using FluentValidation;
using GymManagement.Application.DTOs.Exercises;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExercisesController : BaseApiController
{
    private readonly IExerciseService _exerciseService;
    private readonly IValidator<CreateExerciseDto> _createValidator;
    private readonly IValidator<UpdateExerciseDto> _updateValidator;

    public ExercisesController(
        IExerciseService exerciseService,
        IValidator<CreateExerciseDto> createValidator,
        IValidator<UpdateExerciseDto> updateValidator)
    {
        _exerciseService = exerciseService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll(
        [FromQuery] string? muscleGroup,
        [FromQuery] string? search)
    {
        var exercises = await _exerciseService.GetAllAsync(muscleGroup, search);
        return Ok(exercises);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ExerciseDto>> GetById(int id)
    {
        var exercise = await _exerciseService.GetByIdAsync(id);
        return Ok(exercise);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExerciseDto>> Create([FromBody] CreateExerciseDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var exercise = await _exerciseService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = exercise.Id }, exercise);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExerciseDto>> Update(int id, [FromBody] UpdateExerciseDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var exercise = await _exerciseService.UpdateAsync(id, dto);
        return Ok(exercise);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _exerciseService.DeleteAsync(id);
        return Ok(new { statusCode = 200, message = "Exercise deleted successfully" });
    }
}
