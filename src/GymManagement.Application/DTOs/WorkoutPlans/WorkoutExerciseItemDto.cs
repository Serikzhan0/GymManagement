namespace GymManagementApi.DTOs;

public class WorkoutExerciseItemDto
{
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}
