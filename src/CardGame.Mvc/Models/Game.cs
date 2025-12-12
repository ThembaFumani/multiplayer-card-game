namespace CardGame.Mvc.Models;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<Player>? Players { get; set; } = new();
}

