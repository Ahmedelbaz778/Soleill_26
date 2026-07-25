namespace Soleil.Models.DTOs.Game;

public class GameSuggestionDto
{
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public double FieldScore { get; set; }
}