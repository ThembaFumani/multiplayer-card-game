using CardGame.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardGame.Api.Controllers
{
    [ApiController]
    [Route("games")]
    public class GamesController : ControllerBase
    {
        private readonly IGameServices _gameServices;

        public GamesController(IGameServices gameServices)
        {
            _gameServices = gameServices;
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var game = await _gameServices.CreateGameAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var game = await _gameServices.GetGameAsync(id, ct);
            if (game is null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int skip = 0, [FromQuery] int take = 10, CancellationToken ct = default)
        {
            if (skip < 0 || take <= 0)
            {
                return BadRequest("Skip must be >= 0 and take must be > 0.");
            }

            var (items, total) = await _gameServices.GetGamesAsync(skip, take, ct);
            return Ok(new { items, total });
        }

        [HttpPost("{id:int}/redeal")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Redeal(int id, CancellationToken ct)
        {
            var game = await _gameServices.RedealAsync(id, ct);
            if (game is null)
            {
                return NotFound();
            }

            return Ok(game);
        }
    }
}

