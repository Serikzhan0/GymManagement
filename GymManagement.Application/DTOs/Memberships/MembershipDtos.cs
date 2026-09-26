using GymManagement.Domain.Enums;

namespace GymManagement.Application.DTOs.Memberships;

public class MembershipDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreateMembershipDto
{
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
}

public class UpdateMembershipDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
    public MembershipStatus Status { get; set; }
}
