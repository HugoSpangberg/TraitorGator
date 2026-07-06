using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.Shared.Dtos;

namespace TraitorGator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet("{id}/role")]
    public async Task<ActionResult<PlayerRoleDto>> GetPlayerRole(Guid id)
    {
        var secret = Request.Headers["X-Player-Secret"].FirstOrDefault() ?? string.Empty;
        var role = await _playerService.GetPlayerRoleAsync(id, secret);
        return role == null ? Unauthorized("Spelarsessionen är ogiltig.") : Ok(role);
    }
}
