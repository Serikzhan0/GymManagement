using GymManagementApi.DTOs;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/trainers")]
public class TrainersController : ControllerBase
{
    private readonly ITrainerService _trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TrainerDto>>> GetAll()
    {
        var trainers = await _trainerService.GetAllAsync();
        return Ok(trainers);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<TrainerDto>> GetById(int id)
    {
        var result = await _trainerService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Trainer not found."));
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<TrainerDto>> Create([FromBody] CreateTrainerDto dto)
    {
        var result = await _trainerService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to create trainer.", result.ValidationErrors));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<TrainerDto>> Update(int id, [FromBody] UpdateTrainerDto dto)
    {
        var result = await _trainerService.UpdateAsync(id, dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to update trainer.", result.ValidationErrors));
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _trainerService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to delete trainer."));
        }

        return Ok(new { statusCode = 200, message = result.ErrorMessage ?? "Trainer deleted successfully." });
    }
}
