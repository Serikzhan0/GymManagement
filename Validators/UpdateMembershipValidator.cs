using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class UpdateMembershipValidator : AbstractValidator<UpdateMembershipDto>
{
    public UpdateMembershipValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("StartDate is required.");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Valid membership type is required (Monthly, Quarterly, Yearly).");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Valid membership status is required (Active, Expired, Cancelled, Frozen).");
    }
}
