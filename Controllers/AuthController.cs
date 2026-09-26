using System.Security.Claims;
using BCrypt.Net;
using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        AppDbContext context,
        JwtService jwtService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _context = context;
        _jwtService = jwtService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email);
        if (exists)
        {
            return Conflict(new ErrorResponse(StatusCodes.Status409Conflict, "A user with this email address already exists."));
        }

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return StatusCode(StatusCodes.Status201Created, new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new ErrorResponse(StatusCodes.Status401Unauthorized, "Invalid email or password."));
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }
}
