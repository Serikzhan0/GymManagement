using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/trainers")]
public class TrainersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateTrainerDto> _createValidator;
    private readonly IValidator<UpdateTrainerDto> _updateValidator;

    public TrainersController(
        AppDbContext context,
        IValidator<CreateTrainerDto> createValidator,
        IValidator<UpdateTrainerDto> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TrainerDto>>> GetAll()
    {
        var trainers = await _context.Trainers
            .AsNoTracking()
            .Select(t => new TrainerDto
            {
                Id = t.Id,
                Name = t.Name,
                Specialization = t.Specialization,
                ExperienceYears = t.ExperienceYears
            })
            .ToListAsync();

        return Ok(trainers);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TrainerDto>> GetById(int id)
    {
        var trainer = await _context.Trainers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Trainer with ID '{id}' was not found."));
        }

        return Ok(new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Specialization = trainer.Specialization,
            ExperienceYears = trainer.ExperienceYears
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TrainerDto>> Create([FromBody] CreateTrainerDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var trainer = new Trainer
        {
            Name = dto.Name.Trim(),
            Specialization = dto.Specialization.Trim(),
            ExperienceYears = dto.ExperienceYears
        };

        _context.Trainers.Add(trainer);
        await _context.SaveChangesAsync();

        var result = new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Specialization = trainer.Specialization,
            ExperienceYears = trainer.ExperienceYears
        };

        return CreatedAtAction(nameof(GetById), new { id = trainer.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TrainerDto>> Update(int id, [FromBody] UpdateTrainerDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var trainer = await _context.Trainers.FindAsync(id);
        if (trainer == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Trainer with ID '{id}' was not found."));
        }

        trainer.Name = dto.Name.Trim();
        trainer.Specialization = dto.Specialization.Trim();
        trainer.ExperienceYears = dto.ExperienceYears;

        await _context.SaveChangesAsync();

        return Ok(new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Specialization = trainer.Specialization,
            ExperienceYears = trainer.ExperienceYears
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var trainer = await _context.Trainers
            .Include(t => t.WorkoutPlans)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Trainer with ID '{id}' was not found."));
        }

        if (trainer.WorkoutPlans.Any())
        {
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, "Cannot delete a trainer who has existing workout plans."));
        }

        _context.Trainers.Remove(trainer);
        await _context.SaveChangesAsync();

        return Ok(new { statusCode = 200, message = "Trainer deleted successfully." });
    }
}
