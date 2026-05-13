namespace Soleil.Models.Entities;

public class QuestionnaireField
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    public virtual ICollection<QuestionnaireResult> Results { get; set; } = new List<QuestionnaireResult>();
}