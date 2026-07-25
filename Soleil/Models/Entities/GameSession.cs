namespace Soleil.Models.Entities;

public class GameSession
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int DurationInSeconds { get; set; }
    public bool IsCompleted { get; set; }
    public int SessionNumber { get; set; } 
    public int ChildId { get; set; }
    public virtual Child Child { get; set; } = null!;
    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;
}