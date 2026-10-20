using BCrypt.Net;
using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<AuthResponseDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        var exists = await _userRepository.EmailExistsAsync(email);
        if (exists)
        {
            return ServiceResult<AuthResponseDto>.Failure("A user with this email address already exists.", StatusCodes.Status409Conflict);
        }

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            Balance = user.Balance
        }, StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<AuthResponseDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponseDto>.Failure("Invalid email or password.", StatusCodes.Status401Unauthorized);
        }

        var token = _jwtService.GenerateToken(user);

        return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            Balance = user.Balance
        });
    }
}
