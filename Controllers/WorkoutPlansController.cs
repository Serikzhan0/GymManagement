using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/workout-plans")]
public class WorkoutPlansController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateWorkoutPlanDto> _createValidator;
    private readonly IValidator<UpdateWorkoutPlanDto> _updateValidator;

    public WorkoutPlansController(
        AppDbContext context,
        IValidator<CreateWorkoutPlanDto> createValidator,
        IValidator<UpdateWorkoutPlanDto> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<WorkoutPlanDto>>> GetAll()
    {
        var plans = await _context.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Select(wp => new WorkoutPlanDto
            {
                Id = wp.Id,
                Name = wp.Name,
                Description = wp.Description,
                TrainerId = wp.TrainerId,
                TrainerName = wp.Trainer != null ? wp.Trainer.Name : string.Empty,
                CreatedAt = wp.CreatedAt,
                Exercises = wp.WorkoutExercises.Select(we => new WorkoutExerciseItemDto
                {
                    ExerciseId = we.ExerciseId,
                    ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                    Sets = we.Sets,
                    Repetitions = we.Repetitions
                }).ToList()
            })
            .ToListAsync();

        return Ok(plans);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<WorkoutPlanDto>> GetById(int id)
    {
        var wp = await _context.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(wp => wp.Id == id);

        if (wp == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"WorkoutPlan with ID '{id}' was not found."));
        }

        return Ok(new WorkoutPlanDto
        {
            Id = wp.Id,
            Name = wp.Name,
            Description = wp.Description,
            TrainerId = wp.TrainerId,
            TrainerName = wp.Trainer != null ? wp.Trainer.Name : string.Empty,
            CreatedAt = wp.CreatedAt,
            Exercises = wp.WorkoutExercises.Select(we => new WorkoutExerciseItemDto
            {
                ExerciseId = we.ExerciseId,
                ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                Sets = we.Sets,
                Repetitions = we.Repetitions
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> Create([FromBody] CreateWorkoutPlanDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var trainerExists = await _context.Trainers.AnyAsync(t => t.Id == dto.TrainerId);
        if (!trainerExists)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Trainer with ID '{dto.TrainerId}' was not found."));
        }

        var requestedExerciseIds = dto.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
        var existingCount = await _context.Exercises.CountAsync(e => requestedExerciseIds.Contains(e.Id));
        if (existingCount != requestedExerciseIds.Count)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, "One or more exercises specified were not found."));
        }

        var workoutPlan = new WorkoutPlan
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            TrainerId = dto.TrainerId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var ex in dto.Exercises)
        {
            workoutPlan.WorkoutExercises.Add(new WorkoutExercise
            {
                ExerciseId = ex.ExerciseId,
                Sets = ex.Sets,
                Repetitions = ex.Repetitions
            });
        }

        _context.WorkoutPlans.Add(workoutPlan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workoutPlan.Id }, await GetByIdInternal(workoutPlan.Id));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> Update(int id, [FromBody] UpdateWorkoutPlanDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var plan = await _context.WorkoutPlans
            .Include(wp => wp.WorkoutExercises)
            .FirstOrDefaultAsync(wp => wp.Id == id);

        if (plan == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"WorkoutPlan with ID '{id}' was not found."));
        }

        var requestedExerciseIds = dto.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
        var existingCount = await _context.Exercises.CountAsync(e => requestedExerciseIds.Contains(e.Id));
        if (existingCount != requestedExerciseIds.Count)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, "One or more exercises specified were not found."));
        }

        plan.Name = dto.Name.Trim();
        plan.Description = dto.Description.Trim();

        plan.WorkoutExercises.Clear();
        foreach (var ex in dto.Exercises)
        {
            plan.WorkoutExercises.Add(new WorkoutExercise
            {
                WorkoutPlanId = plan.Id,
                ExerciseId = ex.ExerciseId,
                Sets = ex.Sets,
                Repetitions = ex.Repetitions
            });
        }

        await _context.SaveChangesAsync();

        return Ok(await GetByIdInternal(plan.Id));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _context.WorkoutPlans.FindAsync(id);
        if (plan == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"WorkoutPlan with ID '{id}' was not found."));
        }

        _context.WorkoutPlans.Remove(plan);
        await _context.SaveChangesAsync();

        return Ok(new { statusCode = 200, message = "Workout plan deleted successfully." });
    }

    private async Task<WorkoutPlanDto?> GetByIdInternal(int id)
    {
        return await _context.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Where(wp => wp.Id == id)
            .Select(wp => new WorkoutPlanDto
            {
                Id = wp.Id,
                Name = wp.Name,
                Description = wp.Description,
                TrainerId = wp.TrainerId,
                TrainerName = wp.Trainer != null ? wp.Trainer.Name : string.Empty,
                CreatedAt = wp.CreatedAt,
                Exercises = wp.WorkoutExercises.Select(we => new WorkoutExerciseItemDto
                {
                    ExerciseId = we.ExerciseId,
                    ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                    Sets = we.Sets,
                    Repetitions = we.Repetitions
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
