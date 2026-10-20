using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class WorkoutExerciseInputValidator : AbstractValidator<WorkoutExerciseInputDto>
{
    public WorkoutExerciseInputValidator()
    {
        RuleFor(x => x.ExerciseId).GreaterThan(0).WithMessage("ExerciseId must be greater than 0.");
        RuleFor(x => x.Sets).GreaterThan(0).WithMessage("Sets must be a positive number.");
        RuleFor(x => x.Repetitions).GreaterThan(0).WithMessage("Repetitions must be a positive number.");
    }
}
