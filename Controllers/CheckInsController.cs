using System.Security.Claims;
using GymManagementApi.DTOs;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/check-ins")]
[Route("api/check-in")]
public class CheckInsController : ControllerBase
{
    private readonly ICheckInService _checkInService;

    public CheckInsController(ICheckInService checkInService)
    {
        _checkInService = checkInService;
    }

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CheckInDto>> CheckIn([FromBody] CreateCheckInDto dto)
    {
        var result = await _checkInService.ProcessCheckInAsync(dto, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Check-in failed."));
        }

        return Ok(result.Data);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CheckInDto>>> GetHistory()
    {
        var history = await _checkInService.GetHistoryAsync(CurrentUserId, CurrentUserRole);
        return Ok(history);
    }
}
