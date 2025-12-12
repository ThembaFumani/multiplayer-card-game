namespace CardGame.Mvc.Models;

public class Card
{
    public int Id { get; set; }
    public string Suit { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public int Value { get; set; }
    public int DeckId { get; set; }
    public bool IsJoker { get; set; }
    public int PlayerId { get; set; }
}

