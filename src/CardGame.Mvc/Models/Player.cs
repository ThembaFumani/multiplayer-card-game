namespace CardGame.Mvc.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GameId { get; set; }
    public List<Card>? Cards { get; set; } = new();
    public Score? Score { get; set; }
}

