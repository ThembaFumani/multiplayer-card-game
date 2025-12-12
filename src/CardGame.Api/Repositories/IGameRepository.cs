using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGame.Api.Models;

namespace CardGame.Api.Repositories
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<Game> GetWithDetailsAsync(int id, CancellationToken ct = default);
        Task<(IReadOnlyList<Game> Items, int TotalCount)> GetPagedWithDetailsAsync(
        int skip,
        int take,
        CancellationToken ct = default);
        Task ClearGameStateAsync(Game game, CancellationToken ct = default);
    }
}