using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardGame.Api.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GameId { get; set; }
        public Game? Game { get; set; } = null;
        public ICollection<Card>? Cards { get; set; } = new List<Card>();
        public Score? Score { get; set; } = null;
    }
}