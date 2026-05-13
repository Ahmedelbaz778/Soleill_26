namespace Soleil.Models.Entities;

public class QuestionnaireResult
{
    public int Id { get; set; }
    public double Score { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int ChildId { get; set; }
    public virtual Child Child { get; set; } = null!;
    public int FieldId { get; set; }
    public virtual QuestionnaireField Field { get; set; } = null!;
}