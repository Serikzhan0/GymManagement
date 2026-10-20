namespace GymManagementApi.DTOs;

public class UpdateTrainerDto
{
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}
