namespace GymManagementApi.DTOs;

public class UpdateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
