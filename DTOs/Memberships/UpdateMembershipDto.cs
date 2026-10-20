using GymManagementApi.Models;

namespace GymManagementApi.DTOs;

public class UpdateMembershipDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
    public MembershipStatus Status { get; set; }
}
