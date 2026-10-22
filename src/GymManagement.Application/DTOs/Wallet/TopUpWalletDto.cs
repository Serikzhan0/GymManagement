namespace GymManagementApi.DTOs;

public class TopUpWalletDto
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Card"; // "Card", "QR", "ApplePay"
    public string CardNumber { get; set; } = string.Empty;
}
