using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface IWalletService
{
    Task<ServiceResult<WalletDto>> GetWalletAsync(int userId);
    Task<ServiceResult<WalletDto>> TopUpAsync(int userId, TopUpWalletDto dto);
}
