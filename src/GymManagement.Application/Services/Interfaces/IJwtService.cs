using GymManagementApi.Models;

namespace GymManagementApi.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
