namespace Soleil.Models.Entities;

public class EyeScanTest
{
    public int Id { get; set; }
    public int ChildId { get; set; }

 
    public double AsdProbability { get; set; }
    public double TdProbability { get; set; }
    public string Result { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public int PointsAnalyzed { get; set; }
    public string Decision { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    
    public virtual Child Child { get; set; } = null!;
}