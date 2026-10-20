namespace GymManagementApi.Models;

public class CheckIn
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Granted"; // "Granted" or "Denied"
    public string Notes { get; set; } = string.Empty;
}
