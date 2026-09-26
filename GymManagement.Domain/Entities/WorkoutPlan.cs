namespace GymManagement.Domain.Entities;

public class WorkoutPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Trainer? Trainer { get; set; }
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
