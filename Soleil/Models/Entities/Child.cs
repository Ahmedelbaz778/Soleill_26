namespace Soleil.Models.Entities;

public class Child
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int ParentId { get; set; }
    public virtual Parent Parent { get; set; } = null!;
    public virtual ICollection<EyeScanTest> EyeScanTests { get; set; } = new List<EyeScanTest>();
    public virtual ICollection<QuestionnaireResult> QuestionnaireResults { get; set; } = new List<QuestionnaireResult>();
    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public virtual ChildProgress? ChildProgress { get; set; }
}