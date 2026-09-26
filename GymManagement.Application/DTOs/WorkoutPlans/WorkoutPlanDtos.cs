namespace GymManagement.Application.DTOs.WorkoutPlans;

public class WorkoutExerciseItemDto
{
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}

public class WorkoutExerciseInputDto
{
    public int ExerciseId { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}

public class WorkoutPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<WorkoutExerciseItemDto> Exercises { get; set; } = new();
}

public class CreateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public List<WorkoutExerciseInputDto> Exercises { get; set; } = new();
}

public class UpdateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WorkoutExerciseInputDto> Exercises { get; set; } = new();
}
