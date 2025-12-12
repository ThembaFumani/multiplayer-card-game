using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGame.Api.Data;
using CardGame.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Api.Repositories
{
    public class GameRepository : EfRepository<Game>, IGameRepository
    {
        public GameRepository(CardGameContext context) : base(context)
        {
        }

        public async Task<(IReadOnlyList<Game> Items, int TotalCount)> GetPagedWithDetailsAsync(int skip, int take, CancellationToken ct = default)
        {
            var total = await _context.Games.AsNoTracking().CountAsync(ct);

            var items = await _context.Games
                .AsNoTracking()
                .Include(g => g.Players)
                    .ThenInclude(p => p.Cards)
                .Include(g => g.Players)
                    .ThenInclude(p => p.Score)
                .OrderByDescending(g => g.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<Game> GetWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Games
                .Include(g => g.Players)
                .ThenInclude(p => p.Cards)
                .Include(g => g.Players)
                .ThenInclude(p => p.Score)
                .SingleOrDefaultAsync(g => g.Id == id, ct);
        }

        public async Task ClearGameStateAsync(Game game, CancellationToken ct = default)
        {
            var playerIds = await _context.Players
                .Where(p => p.GameId == game.Id)
                .Select(p => p.Id)
                .ToListAsync(ct);

            if (playerIds.Count == 0)
            {
                return;
            }

            var cards = _context.Cards.Where(c => playerIds.Contains(c.PlayerId));
            var scores = _context.Scores.Where(s => playerIds.Contains(s.PlayerId));

            _context.Cards.RemoveRange(cards);
            _context.Scores.RemoveRange(scores);
        }
    }
}