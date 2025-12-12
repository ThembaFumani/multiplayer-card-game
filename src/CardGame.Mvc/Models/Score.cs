namespace CardGame.Mvc.Models;

public class Score
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int HandSum { get; set; }
    public int SuitProduct { get; set; }
    public bool IsWinner { get; set; }
}

