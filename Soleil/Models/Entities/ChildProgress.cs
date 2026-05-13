namespace Soleil.Models.Entities;

public class ChildProgress
{
    public int Id { get; set; }
    public double SocialPercentage { get; set; }
    public double CommunicationPercentage { get; set; }
    public double SkillsPercentage { get; set; }
    public DateTime LastUpdated { get; set; }
    public int ChildId { get; set; }
    public virtual Child Child { get; set; } = null!;
}