using GymManagement.Application.DTOs.Memberships;

namespace GymManagement.Application.Interfaces;

public interface IMembershipService
{
    Task<IEnumerable<MembershipDto>> GetAllAsync(int currentUserId, string currentUserRole);
    Task<MembershipDto> GetByIdAsync(int id, int currentUserId, string currentUserRole);
    Task<MembershipDto> CreateAsync(CreateMembershipDto dto, int currentUserId, string currentUserRole);
    Task<MembershipDto> UpdateAsync(int id, UpdateMembershipDto dto, int currentUserId, string currentUserRole);
    Task DeleteAsync(int id, int currentUserId, string currentUserRole);
}
