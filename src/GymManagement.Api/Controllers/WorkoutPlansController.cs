using GymManagementApi.DTOs;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/workout-plans")]
public class WorkoutPlansController : ControllerBase
{
    private readonly IWorkoutPlanService _workoutPlanService;

    public WorkoutPlansController(IWorkoutPlanService workoutPlanService)
    {
        _workoutPlanService = workoutPlanService;
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
        var result = await _workoutPlanService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "WorkoutPlan not found."));
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> Create([FromBody] CreateWorkoutPlanDto dto)
    {
        var result = await _workoutPlanService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to create workout plan.", result.ValidationErrors));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> Update(int id, [FromBody] UpdateWorkoutPlanDto dto)
    {
        var result = await _workoutPlanService.UpdateAsync(id, dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to update workout plan.", result.ValidationErrors));
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _workoutPlanService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to delete workout plan."));
        }

        return Ok(new { statusCode = 200, message = result.ErrorMessage ?? "Workout plan deleted successfully." });
    }
}
