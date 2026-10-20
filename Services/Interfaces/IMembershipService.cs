using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface IMembershipService
{
    Task AutoExpireMembershipsAsync(int? userId = null);
    Task<IEnumerable<MembershipDto>> GetAllAsync(int currentUserId, string currentUserRole);
    Task<ServiceResult<MembershipDto>> GetByIdAsync(int id, int currentUserId, string currentUserRole);
    Task<ServiceResult<MembershipDto>> CreateAsync(CreateMembershipDto dto, int currentUserId, string currentUserRole);
    Task<ServiceResult<MembershipDto>> UpdateAsync(int id, UpdateMembershipDto dto, int currentUserId, string currentUserRole);
    Task<ServiceResult> DeleteAsync(int id, int currentUserId, string currentUserRole);
    Task<ServiceResult<MembershipDto>> FreezeAsync(int id, int currentUserId, string currentUserRole);
    Task<ServiceResult<MembershipDto>> UnfreezeAsync(int id, int currentUserId, string currentUserRole);
}
