using FluentValidation;
using GymManagement.Application.DTOs.WorkoutPlans;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/workout-plans")]
public class WorkoutPlansController : BaseApiController
{
    private readonly IWorkoutPlanService _workoutPlanService;
    private readonly IValidator<CreateWorkoutPlanDto> _createValidator;
    private readonly IValidator<UpdateWorkoutPlanDto> _updateValidator;

    public WorkoutPlansController(
        IWorkoutPlanService workoutPlanService,
        IValidator<CreateWorkoutPlanDto> createValidator,
        IValidator<UpdateWorkoutPlanDto> updateValidator)
    {
        _workoutPlanService = workoutPlanService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<WorkoutPlanDto>>> GetAll()
    {
        var plans = await _workoutPlanService.GetAllAsync();
        return Ok(plans);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<WorkoutPlanDto>> GetById(int id)
    {
        var plan = await _workoutPlanService.GetByIdAsync(id);
        return Ok(plan);
    }

    [HttpPost]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> Create([FromBody] CreateWorkoutPlanDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var plan = await _workoutPlanService.CreateAsync(dto, CurrentUserId, CurrentUserRole);
        return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> Update(int id, [FromBody] UpdateWorkoutPlanDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var plan = await _workoutPlanService.UpdateAsync(id, dto, CurrentUserId, CurrentUserRole);
        return Ok(plan);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workoutPlanService.DeleteAsync(id, CurrentUserId, CurrentUserRole);
        return Ok(new { statusCode = 200, message = "Workout plan deleted successfully" });
    }
}
