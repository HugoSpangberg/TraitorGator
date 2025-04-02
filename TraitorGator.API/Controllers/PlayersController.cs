using Microsoft.AspNetCore.Mvc;
using TraitorGator.Services.Interfaces;
using TraitorGator.API.Models;
using TraitorGator.Models.Models;

namespace TraitorGator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        // Lägg till en spelare till en spelrunda
        [HttpPost("game/{gameRoundId}")]
        public async Task<ActionResult<Player>> AddPlayerToGame(string gameCode, [FromBody] string username)
        {
            var player = await _playerService.AddPlayerToGameAsync(gameCode, username);
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        // Hämta en spelare med id
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(Guid id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return player;
        }

        // Tilldela en roll till en spelare
        [HttpPut("{id}/role")]
        public async Task<ActionResult<Player>> AssignRoleToPlayer(Guid id, [FromBody] PlayerRole role)
        {
            var player = await _playerService.AssignRoleAsync(id, role);
            return player;
        }
    }
}
