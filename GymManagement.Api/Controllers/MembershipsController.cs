using FluentValidation;
using GymManagement.Application.DTOs.Memberships;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/memberships")]
public class MembershipsController : BaseApiController
{
    private readonly IMembershipService _membershipService;
    private readonly IValidator<CreateMembershipDto> _createValidator;
    private readonly IValidator<UpdateMembershipDto> _updateValidator;

    public MembershipsController(
        IMembershipService membershipService,
        IValidator<CreateMembershipDto> createValidator,
        IValidator<UpdateMembershipDto> updateValidator)
    {
        _membershipService = membershipService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MembershipDto>>> GetAll()
    {
        var memberships = await _membershipService.GetAllAsync(CurrentUserId, CurrentUserRole);
        return Ok(memberships);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> GetById(int id)
    {
        var membership = await _membershipService.GetByIdAsync(id, CurrentUserId, CurrentUserRole);
        return Ok(membership);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Create([FromBody] CreateMembershipDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var membership = await _membershipService.CreateAsync(dto, CurrentUserId, CurrentUserRole);
        return CreatedAtAction(nameof(GetById), new { id = membership.Id }, membership);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Update(int id, [FromBody] UpdateMembershipDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var membership = await _membershipService.UpdateAsync(id, dto, CurrentUserId, CurrentUserRole);
        return Ok(membership);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _membershipService.DeleteAsync(id, CurrentUserId, CurrentUserRole);
        return Ok(new { statusCode = 200, message = "Membership deleted successfully" });
    }
}
