using FluentValidation;
using GymManagement.Application.DTOs.Trainers;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/trainers")]
public class TrainersController : BaseApiController
{
    private readonly ITrainerService _trainerService;
    private readonly IValidator<CreateTrainerDto> _createValidator;
    private readonly IValidator<UpdateTrainerDto> _updateValidator;

    public TrainersController(
        ITrainerService trainerService,
        IValidator<CreateTrainerDto> createValidator,
        IValidator<UpdateTrainerDto> updateValidator)
    {
        _trainerService = trainerService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TrainerDto>>> GetAll()
    {
        var trainers = await _trainerService.GetAllAsync();
        return Ok(trainers);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TrainerDto>> GetById(int id)
    {
        var trainer = await _trainerService.GetByIdAsync(id);
        return Ok(trainer);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TrainerDto>> Create([FromBody] CreateTrainerDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var trainer = await _trainerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = trainer.Id }, trainer);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TrainerDto>> Update(int id, [FromBody] UpdateTrainerDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var trainer = await _trainerService.UpdateAsync(id, dto);
        return Ok(trainer);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _trainerService.DeleteAsync(id);
        return Ok(new { statusCode = 200, message = "Trainer deleted successfully" });
    }
}
