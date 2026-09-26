namespace GymManagementApi.Models;

public class WorkoutExercise
{
    public int WorkoutPlanId { get; set; }
    public WorkoutPlan? WorkoutPlan { get; set; }

    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public int Sets { get; set; }
    public int Repetitions { get; set; }
}
