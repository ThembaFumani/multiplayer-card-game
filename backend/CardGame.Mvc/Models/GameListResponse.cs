namespace CardGame.Mvc.Models;

public class GameListResponse
{
    public List<Game> Items { get; set; } = new();
    public int Total { get; set; }
}

