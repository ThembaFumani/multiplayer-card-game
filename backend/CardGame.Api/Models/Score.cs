using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardGame.Api.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; } = null;
        public int HandSum { get; set; }   
        public int SuitProduct { get; set; } 
        public bool IsWinner { get; set; }
    }
}