using System.Security.Claims;
using GymManagementApi.DTOs;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/wallet")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    [HttpGet]
    public async Task<ActionResult<WalletDto>> GetWallet()
    {
        var result = await _walletService.GetWalletAsync(CurrentUserId);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Wallet not found."));
        }

        return Ok(result.Data);
    }

    [HttpPost("topup")]
    public async Task<ActionResult<WalletDto>> TopUp([FromBody] TopUpWalletDto dto)
    {
        var result = await _walletService.TopUpAsync(CurrentUserId, dto);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new ErrorResponse(result.StatusCode, result.ErrorMessage ?? "Top up failed."));
        }

        return Ok(result.Data);
    }
}
