using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateExerciseDto> _createValidator;
    private readonly IValidator<UpdateExerciseDto> _updateValidator;

    public ExercisesController(
        AppDbContext context,
        IValidator<CreateExerciseDto> createValidator,
        IValidator<UpdateExerciseDto> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll(
        [FromQuery] string? muscleGroup,
        [FromQuery] string? search)
    {
        var query = _context.Exercises.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            var mg = muscleGroup.Trim().ToLower();
            query = query.Where(e => e.MuscleGroup.ToLower() == mg);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(s) || e.Description.ToLower().Contains(s));
        }

        var exercises = await query
            .Select(e => new ExerciseDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                MuscleGroup = e.MuscleGroup
            })
            .ToListAsync();

        return Ok(exercises);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ExerciseDto>> GetById(int id)
    {
        var exercise = await _context.Exercises
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Exercise with ID '{id}' was not found."));
        }

        return Ok(new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            MuscleGroup = exercise.MuscleGroup
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExerciseDto>> Create([FromBody] CreateExerciseDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var exercise = new Exercise
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            MuscleGroup = dto.MuscleGroup.Trim()
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        var result = new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            MuscleGroup = exercise.MuscleGroup
        };

        return CreatedAtAction(nameof(GetById), new { id = exercise.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExerciseDto>> Update(int id, [FromBody] UpdateExerciseDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Exercise with ID '{id}' was not found."));
        }

        exercise.Name = dto.Name.Trim();
        exercise.Description = dto.Description.Trim();
        exercise.MuscleGroup = dto.MuscleGroup.Trim();

        await _context.SaveChangesAsync();

        return Ok(new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            MuscleGroup = exercise.MuscleGroup
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var exercise = await _context.Exercises
            .Include(e => e.WorkoutExercises)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Exercise with ID '{id}' was not found."));
        }

        if (exercise.WorkoutExercises.Any())
        {
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, "Cannot delete an exercise that is currently in workout plans."));
        }

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return Ok(new { statusCode = 200, message = "Exercise deleted successfully." });
    }
}
