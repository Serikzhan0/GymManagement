using FluentValidation;
using GymManagement.Application.DTOs.Exercises;

namespace GymManagement.Application.Validators;

public class CreateExerciseValidator : AbstractValidator<CreateExerciseDto>
{
    public CreateExerciseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exercise name is required.")
            .MaximumLength(100).WithMessage("Exercise name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.MuscleGroup)
            .NotEmpty().WithMessage("Muscle group is required.")
            .MaximumLength(50).WithMessage("Muscle group cannot exceed 50 characters.");
    }
}

public class UpdateExerciseValidator : AbstractValidator<UpdateExerciseDto>
{
    public UpdateExerciseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exercise name is required.")
            .MaximumLength(100).WithMessage("Exercise name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.MuscleGroup)
            .NotEmpty().WithMessage("Muscle group is required.")
            .MaximumLength(50).WithMessage("Muscle group cannot exceed 50 characters.");
    }
}
