using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
