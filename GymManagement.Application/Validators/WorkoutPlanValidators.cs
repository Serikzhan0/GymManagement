using FluentValidation;
using GymManagement.Application.DTOs.WorkoutPlans;

namespace GymManagement.Application.Validators;

public class WorkoutExerciseInputValidator : AbstractValidator<WorkoutExerciseInputDto>
{
    public WorkoutExerciseInputValidator()
    {
        RuleFor(x => x.ExerciseId)
            .GreaterThan(0).WithMessage("ExerciseId must be greater than 0.");

        RuleFor(x => x.Sets)
            .GreaterThan(0).WithMessage("Sets must be a positive number.");

        RuleFor(x => x.Repetitions)
            .GreaterThan(0).WithMessage("Repetitions must be a positive number.");
    }
}

public class CreateWorkoutPlanValidator : AbstractValidator<CreateWorkoutPlanDto>
{
    public CreateWorkoutPlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workout plan name is required.")
            .MaximumLength(100).WithMessage("Workout plan name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.TrainerId)
            .GreaterThan(0).WithMessage("TrainerId must be greater than 0.");

        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("A workout plan must contain at least one exercise.")
            .Must(exercises => exercises != null && exercises.Select(e => e.ExerciseId).Distinct().Count() == exercises.Count)
            .WithMessage("The same exercise cannot be added twice to the same workout plan.");

        RuleForEach(x => x.Exercises)
            .SetValidator(new WorkoutExerciseInputValidator());
    }
}

public class UpdateWorkoutPlanValidator : AbstractValidator<UpdateWorkoutPlanDto>
{
    public UpdateWorkoutPlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workout plan name is required.")
            .MaximumLength(100).WithMessage("Workout plan name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("A workout plan must contain at least one exercise.")
            .Must(exercises => exercises != null && exercises.Select(e => e.ExerciseId).Distinct().Count() == exercises.Count)
            .WithMessage("The same exercise cannot be added twice to the same workout plan.");

        RuleForEach(x => x.Exercises)
            .SetValidator(new WorkoutExerciseInputValidator());
    }
}
