using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class WalletService : IWalletService
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;

    public WalletService(IUserRepository userRepository, IWalletRepository walletRepository)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
    }

    public async Task<ServiceResult<WalletDto>> GetWalletAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<WalletDto>.Failure("User not found.", StatusCodes.Status404NotFound);
        }

        var transactions = await _walletRepository.GetTransactionsByUserIdAsync(user.Id, 30);

        return ServiceResult<WalletDto>.Success(new WalletDto
        {
            Balance = user.Balance,
            Transactions = transactions.Select(MapTransactionToDto).ToList()
        });
    }

    public async Task<ServiceResult<WalletDto>> TopUpAsync(int userId, TopUpWalletDto dto)
    {
        if (dto.Amount <= 0 || dto.Amount > 10000)
        {
            return ServiceResult<WalletDto>.Failure("Сумма пополнения должна быть от $1 до $10,000.", StatusCodes.Status400BadRequest);
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<WalletDto>.Failure("User not found.", StatusCodes.Status404NotFound);
        }

        user.Balance += dto.Amount;

        var mask = !string.IsNullOrWhiteSpace(dto.CardNumber) && dto.CardNumber.Length >= 4
            ? $" (•••• {dto.CardNumber[^4..]})"
            : dto.PaymentMethod == "QR" ? " (Kaspi/Click QR)" : " (Apple Pay)";

        var tx = new WalletTransaction
        {
            UserId = user.Id,
            Amount = dto.Amount,
            Description = $"Пополнение баланса{mask}",
            Type = "TopUp",
            CreatedAt = DateTime.UtcNow
        };

        await _walletRepository.AddTransactionAsync(tx);
        await _userRepository.UpdateAsync(user);
        await _walletRepository.SaveChangesAsync();

        var recentTxs = await _walletRepository.GetTransactionsByUserIdAsync(user.Id, 30);

        return ServiceResult<WalletDto>.Success(new WalletDto
        {
            Balance = user.Balance,
            Transactions = recentTxs.Select(MapTransactionToDto).ToList()
        });
    }

    private static WalletTransactionDto MapTransactionToDto(WalletTransaction t) => new()
    {
        Id = t.Id,
        Amount = t.Amount,
        Description = t.Description,
        Type = t.Type,
        CreatedAt = t.CreatedAt
    };
}
