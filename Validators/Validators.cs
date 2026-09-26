using FluentValidation;
using GymManagementApi.DTOs;

namespace GymManagementApi.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Valid email format is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.Role).IsInEnum().WithMessage("Invalid role specified.");
    }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Valid email format is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}

public class CreateTrainerValidator : AbstractValidator<CreateTrainerDto>
{
    public CreateTrainerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Trainer name is required.").MaximumLength(100);
        RuleFor(x => x.Specialization).NotEmpty().WithMessage("Specialization is required.").MaximumLength(100);
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0).WithMessage("Experience years must be positive or zero.");
    }
}

public class UpdateTrainerValidator : AbstractValidator<UpdateTrainerDto>
{
    public UpdateTrainerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Trainer name is required.").MaximumLength(100);
        RuleFor(x => x.Specialization).NotEmpty().WithMessage("Specialization is required.").MaximumLength(100);
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0).WithMessage("Experience years must be positive or zero.");
    }
}

public class CreateExerciseValidator : AbstractValidator<CreateExerciseDto>
{
    public CreateExerciseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Exercise name is required.").MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(500);
        RuleFor(x => x.MuscleGroup).NotEmpty().WithMessage("Muscle group is required.").MaximumLength(50);
    }
}

public class UpdateExerciseValidator : AbstractValidator<UpdateExerciseDto>
{
    public UpdateExerciseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Exercise name is required.").MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(500);
        RuleFor(x => x.MuscleGroup).NotEmpty().WithMessage("Muscle group is required.").MaximumLength(50);
    }
}

public class WorkoutExerciseInputValidator : AbstractValidator<WorkoutExerciseInputDto>
{
    public WorkoutExerciseInputValidator()
    {
        RuleFor(x => x.ExerciseId).GreaterThan(0).WithMessage("ExerciseId must be greater than 0.");
        RuleFor(x => x.Sets).GreaterThan(0).WithMessage("Sets must be a positive number.");
        RuleFor(x => x.Repetitions).GreaterThan(0).WithMessage("Repetitions must be a positive number.");
    }
}

public class CreateWorkoutPlanValidator : AbstractValidator<CreateWorkoutPlanDto>
{
    public CreateWorkoutPlanValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Workout plan name is required.").MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(500);
        RuleFor(x => x.TrainerId).GreaterThan(0).WithMessage("TrainerId must be greater than 0.");
        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("A workout plan must contain at least one exercise.")
            .Must(e => e != null && e.Select(x => x.ExerciseId).Distinct().Count() == e.Count)
            .WithMessage("The same exercise cannot be added twice to one workout plan.");

        RuleForEach(x => x.Exercises).SetValidator(new WorkoutExerciseInputValidator());
    }
}

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

public class UpdateMembershipValidator : AbstractValidator<UpdateMembershipDto>
{
    public UpdateMembershipValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("StartDate is required.");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Valid membership type is required (Monthly, Quarterly, Yearly).");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Valid membership status is required (Active, Expired, Cancelled).");
    }
}
