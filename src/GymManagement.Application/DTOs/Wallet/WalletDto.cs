namespace GymManagementApi.DTOs;

public class WalletDto
{
    public decimal Balance { get; set; }
    public List<WalletTransactionDto> Transactions { get; set; } = new();
}
