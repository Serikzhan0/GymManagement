using System.Security.Claims;
using GymManagementApi.DTOs;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/memberships")]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipService _membershipService;

    public MembershipsController(IMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MembershipDto>>> GetAll()
    {
        var list = await _membershipService.GetAllAsync(CurrentUserId, CurrentUserRole);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> GetById(int id)
    {
        var result = await _membershipService.GetByIdAsync(id, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Membership not found."));
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Create([FromBody] CreateMembershipDto dto)
    {
        var result = await _membershipService.CreateAsync(dto, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to create membership.", result.ValidationErrors));
        }

        if (result.StatusCode == StatusCodes.Status201Created && result.Data != null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        return Ok(result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Update(int id, [FromBody] UpdateMembershipDto dto)
    {
        var result = await _membershipService.UpdateAsync(id, dto, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to update membership.", result.ValidationErrors));
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _membershipService.DeleteAsync(id, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to delete membership."));
        }

        return Ok(new { statusCode = 200, message = result.ErrorMessage ?? "Membership deleted successfully." });
    }

    [HttpPost("{id:int}/freeze")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Freeze(int id)
    {
        var result = await _membershipService.FreezeAsync(id, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to freeze membership."));
        }

        return Ok(result.Data);
    }

    [HttpPost("{id:int}/unfreeze")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Unfreeze(int id)
    {
        var result = await _membershipService.UnfreezeAsync(id, CurrentUserId, CurrentUserRole);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Failed to unfreeze membership."));
        }

        return Ok(result.Data);
    }
}
