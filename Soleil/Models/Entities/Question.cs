namespace Soleil.Models.Entities;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int FieldId { get; set; }
    public virtual QuestionnaireField Field { get; set; } = null!;
}