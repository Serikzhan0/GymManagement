using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class UpdateTrainerValidator : AbstractValidator<UpdateTrainerDto>
{
    public UpdateTrainerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Trainer name is required.").MaximumLength(100);
        RuleFor(x => x.Specialization).NotEmpty().WithMessage("Specialization is required.").MaximumLength(100);
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0).WithMessage("Experience years must be positive or zero.");
    }
}
