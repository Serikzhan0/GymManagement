using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class UpdateExerciseValidator : AbstractValidator<UpdateExerciseDto>
{
    public UpdateExerciseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Exercise name is required.").MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(500);
        RuleFor(x => x.MuscleGroup).NotEmpty().WithMessage("Muscle group is required.").MaximumLength(50);
    }
}
