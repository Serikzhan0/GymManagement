using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class UpdateWorkoutPlanValidator : AbstractValidator<UpdateWorkoutPlanDto>
{
    public UpdateWorkoutPlanValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Workout plan name is required.").MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(500);
        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("A workout plan must contain at least one exercise.")
            .Must(e => e != null && e.Select(x => x.ExerciseId).Distinct().Count() == e.Count)
            .WithMessage("The same exercise cannot be added twice to one workout plan.");

        RuleForEach(x => x.Exercises).SetValidator(new WorkoutExerciseInputValidator());
    }
}
