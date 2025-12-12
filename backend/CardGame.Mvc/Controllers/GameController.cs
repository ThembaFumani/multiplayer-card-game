using CardGame.Mvc.Models;
using CardGame.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardGame.Mvc.Controllers;

public class GameController : Controller
{
    private readonly GameApiService _apiService;
    private readonly ILogger<GameController> _logger;

    public GameController(GameApiService apiService, ILogger<GameController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? id, CancellationToken ct = default)
    {
        try
        {
            if (id.HasValue)
            {
                var game = await _apiService.GetGameAsync(id.Value, ct);
                if (game == null)
                {
                    TempData["Error"] = "Game not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(game);
            }
            else
            {
                var game = await _apiService.CreateGameAsync(ct);
                return RedirectToAction(nameof(Index), new { id = game.Id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading game");
            TempData["Error"] = $"Failed to load game: {ex.Message}";
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        try
        {
            var game = await _apiService.CreateGameAsync(ct);
            return RedirectToAction(nameof(Index), new { id = game.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            TempData["Error"] = $"Failed to create game: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Redeal(int id, CancellationToken ct = default)
    {
        try
        {
            var game = await _apiService.RedealAsync(id, ct);
            if (game == null)
            {
                TempData["Error"] = "Game not found.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index), new { id = game.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redealing game");
            TempData["Error"] = $"Failed to redeal: {ex.Message}";
            return RedirectToAction(nameof(Index), new { id });
        }
    }
}

