namespace GymManagementApi.DTOs;

public class UpdateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WorkoutExerciseInputDto> Exercises { get; set; } = new();
}
