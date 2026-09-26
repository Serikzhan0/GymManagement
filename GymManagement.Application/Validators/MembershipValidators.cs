using FluentValidation;
using GymManagement.Application.DTOs.Memberships;

namespace GymManagement.Application.Validators;

public class CreateMembershipValidator : AbstractValidator<CreateMembershipDto>
{
    public CreateMembershipValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid membership type. Allowed values: Monthly, Quarterly, Yearly.");
    }
}

public class UpdateMembershipValidator : AbstractValidator<UpdateMembershipDto>
{
    public UpdateMembershipValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid membership type. Allowed values: Monthly, Quarterly, Yearly.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid membership status. Allowed values: Active, Expired, Cancelled.");
    }
}
