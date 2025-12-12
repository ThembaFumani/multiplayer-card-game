using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGame.Api.Models;

namespace CardGame.Api.Services
{
    public interface IGameServices
    {
        Task<Game> CreateGameAsync(CancellationToken ct = default);
        Task<Game?> GetGameAsync(int id, CancellationToken ct = default);
        Task<(IEnumerable<Game> Items, int TotalCount)> GetGamesAsync(int skip, int take, CancellationToken ct = default);
        Task<Game?> RedealAsync(int gameId, CancellationToken ct = default);
    }
}