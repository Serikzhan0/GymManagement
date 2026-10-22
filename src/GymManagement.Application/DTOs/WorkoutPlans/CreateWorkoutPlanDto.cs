namespace GymManagementApi.DTOs;

public class CreateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public List<WorkoutExerciseInputDto> Exercises { get; set; } = new();
}
