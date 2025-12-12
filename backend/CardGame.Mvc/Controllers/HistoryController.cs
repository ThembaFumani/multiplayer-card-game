using CardGame.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardGame.Mvc.Controllers;

public class HistoryController : Controller
{
    private readonly GameApiService _apiService;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(GameApiService apiService, ILogger<HistoryController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int skip = 0, int take = 10, CancellationToken ct = default)
    {
        try
        {
            if (skip < 0) skip = 0;
            if (take <= 0) take = 10;

            var response = await _apiService.GetGamesAsync(skip, take, ct);
            ViewBag.Skip = skip;
            ViewBag.Take = take;
            ViewBag.Total = response.Total;
            ViewBag.CurrentPage = (skip / take) + 1;
            ViewBag.TotalPages = (int)Math.Ceiling((double)response.Total / take);

            return View(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading game history");
            TempData["Error"] = $"Failed to load history: {ex.Message}";
            return View(new Models.GameListResponse());
        }
    }
}

