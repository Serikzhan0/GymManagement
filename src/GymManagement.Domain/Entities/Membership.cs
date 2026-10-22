namespace GymManagementApi.Models;

public class Membership
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
    public MembershipStatus Status { get; set; }

    public User? User { get; set; }
}
