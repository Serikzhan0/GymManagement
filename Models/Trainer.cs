namespace GymManagementApi.Models;

public class Trainer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public double Rating { get; set; } = 4.9;
    public int SessionsCount { get; set; } = 120;
    public int ReviewsCount { get; set; } = 45;

    public List<WorkoutPlan> WorkoutPlans { get; set; } = new();
}
