using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardGame.Api.Models;
using CardGame.Api.Repositories;

namespace CardGame.Api.Services
{
    public class GameService : IGameServices
    {
        private readonly IGameRepository _gameRepository;
        private readonly Random _random = new();

        private static readonly string[] Suits = new[] { "♦", "♥", "♠", "♣" };
        private static readonly string[] Ranks = new[]
        {
            "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "J", "Q", "K", "A"
        };

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<Game> CreateGameAsync(CancellationToken ct = default)
        {
            var game = new Game
            {
                Name = $"Game {DateTime.UtcNow:yyyyMMddHHmmss}",
                CreatedAt = DateTime.UtcNow,
                Players = new List<Player>()
            };

            DealHands(game);

            await _gameRepository.AddAsync(game, ct);
            await _gameRepository.SaveChangesAsync(ct);

            return game;
        }

        public Task<Game?> GetGameAsync(int id, CancellationToken ct = default)
        {
            return _gameRepository.GetWithDetailsAsync(id, ct);
        }

        public async Task<(IEnumerable<Game> Items, int TotalCount)> GetGamesAsync(int skip, int take, CancellationToken ct = default)
        {
            var result = await _gameRepository.GetPagedWithDetailsAsync(skip, take, ct);
            return (result.Items, result.TotalCount);
        }

        public async Task<Game?> RedealAsync(int gameId, CancellationToken ct = default)
        {
            var existing = await _gameRepository.GetWithDetailsAsync(gameId, ct);
            if (existing == null)
            {
                return null;
            }

            existing.Players ??= new List<Player>();
            if (!existing.Players.Any())
            {
                for (var i = 1; i <= 6; i++)
                {
                    existing.Players.Add(new Player { Name = $"Player {i}" });
                }
            }

            await _gameRepository.ClearGameStateAsync(existing, ct);
            DealHands(existing, resetExisting: true);
            existing.CreatedAt = DateTime.UtcNow;

            await _gameRepository.SaveChangesAsync(ct);
            return existing;
        }

        private void DealHands(Game game, bool resetExisting = false)
        {
            var deck = BuildShuffledDeck();
            var deckQueue = new Queue<Card>(deck);

            if (resetExisting)
            {
                foreach (var player in game.Players!)
                {
                    player.Cards = new List<Card>();
                    player.Score = null;
                }
            }
            else
            {
                game.Players ??= new List<Player>();
                if (!game.Players.Any())
                {
                    for (var i = 1; i <= 6; i++)
                    {
                        game.Players.Add(new Player { Name = $"Player {i}" });
                    }
                }
            }

            foreach (var player in game.Players!)
            {
                var hand = new List<Card>();
                for (var i = 0; i < 5; i++)
                {
                    if (!deckQueue.TryDequeue(out var card))
                    {
                        throw new InvalidOperationException("Deck exhausted unexpectedly.");
                    }
                    hand.Add(card);
                }

                var handSum = hand.Sum(c => c.Value);
                var suitProduct = hand.Aggregate(1, (acc, c) => acc * SuitValue(c.Suit));

                player.Cards = hand;
                player.Score = new Score
                {
                    HandSum = handSum,
                    SuitProduct = suitProduct,
                    IsWinner = false
                };
            }

            ApplyWinners(game.Players!);
        }

        private List<Card> BuildShuffledDeck()
        {
            var deck = new List<Card>(104);

            for (var deckId = 1; deckId <= 2; deckId++)
            {
                foreach (var suit in Suits)
                {
                    foreach (var rank in Ranks)
                    {
                        deck.Add(new Card
                        {
                            Suit = suit,
                            Rank = rank,
                            Value = RankValue(rank),
                            DeckId = deckId,
                            IsJoker = false
                        });
                    }
                }
            }

            return deck.OrderBy(_ => _random.Next()).ToList();
        }

        private static int RankValue(string rank)
        {
            return rank switch
            {
                "J" => 11,
                "Q" => 12,
                "K" => 13,
                "A" => 11,
                _ => int.TryParse(rank, out var n) ? n : 0
            };
        }

        private static int SuitValue(string suit)
        {
            return suit switch
            {
                "♦" => 1,
                "♥" => 2,
                "♠" => 3,
                "♣" => 4,
                _ => 0
            };
        }

        private static void ApplyWinners(IEnumerable<Player> players)
        {
            var playerList = players.ToList();
            if (!playerList.Any())
            {
                return;
            }

            var topHand = playerList.Max(p => p.Score?.HandSum ?? 0);
            var handLeaders = playerList.Where(p => (p.Score?.HandSum ?? 0) == topHand).ToList();

            if (handLeaders.Count == 1)
            {
                handLeaders[0].Score!.IsWinner = true;
                return;
            }

            var topSuit = handLeaders.Max(p => p.Score?.SuitProduct ?? 0);
            var finalWinners = handLeaders.Where(p => (p.Score?.SuitProduct ?? 0) == topSuit).ToList();

            foreach (var winner in finalWinners)
            {
                winner.Score!.IsWinner = true;
            }
        }
    }
}