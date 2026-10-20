using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Valid email format is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}
