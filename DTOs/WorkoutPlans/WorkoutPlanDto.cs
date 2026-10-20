namespace GymManagementApi.DTOs;

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
