namespace Soleil.Models.DTOs.Game;

public class StartSessionDto
{
    public int ChildId { get; set; }
    public int GameId { get; set; }
}

public class CompleteSessionDto
{
    public int SessionId { get; set; }
    public int DurationInSeconds { get; set; }
}

public class SessionStatusDto
{
    public int CompletedSessions { get; set; }
    public int TotalSessions { get; set; }
    public bool ReTestRequired { get; set; }
    public string Message { get; set; } = string.Empty;
}