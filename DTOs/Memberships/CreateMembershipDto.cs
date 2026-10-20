using GymManagementApi.Models;

namespace GymManagementApi.DTOs;

public class CreateMembershipDto
{
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
}
