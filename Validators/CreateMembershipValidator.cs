using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class CreateMembershipValidator : AbstractValidator<CreateMembershipDto>
{
    public CreateMembershipValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0.");
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("StartDate is required.");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Valid membership type is required (Monthly, Quarterly, Yearly).");
    }
}
