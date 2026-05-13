namespace Soleil.Models.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FieldId { get; set; }
    public virtual QuestionnaireField Field { get; set; } = null!;
}