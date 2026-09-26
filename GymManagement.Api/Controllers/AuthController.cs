using FluentValidation;
using GymManagement.Application.DTOs.Auth;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    public AuthController(
        IAuthService _authService,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator)
    {
        this._authService = _authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var result = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            var firstMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return BadRequest(new { statusCode = 400, message = firstMessage, errors });
        }

        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
}
