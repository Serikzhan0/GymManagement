namespace GymManagementApi.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public decimal Balance { get; set; } = 150.0m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Membership> Memberships { get; set; } = new();
    public List<CheckIn> CheckIns { get; set; } = new();
    public List<WalletTransaction> WalletTransactions { get; set; } = new();
}
