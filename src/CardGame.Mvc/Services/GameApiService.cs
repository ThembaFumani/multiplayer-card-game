using System.Net.Http.Json;
using CardGame.Mvc.Models;

namespace CardGame.Mvc.Services;

public class GameApiService
{
    private readonly HttpClient _httpClient;

    public GameApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Game> CreateGameAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync("/games", null, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Game>(ct) 
            ?? throw new InvalidOperationException("Failed to deserialize game response.");
    }

    public async Task<Game?> GetGameAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Game>($"/games/{id}", ct);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            return null;
        }
    }

    public async Task<GameListResponse> GetGamesAsync(int skip = 0, int take = 10, CancellationToken ct = default)
    {
        var response = await _httpClient.GetFromJsonAsync<GameListResponse>($"/games?skip={skip}&take={take}", ct);
        return response ?? new GameListResponse();
    }

    public async Task<Game?> RedealAsync(int gameId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/games/{gameId}/redeal", null, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Game>(ct);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            return null;
        }
    }
}

