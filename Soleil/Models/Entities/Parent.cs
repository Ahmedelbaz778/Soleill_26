namespace Soleil.Models.Entities;

public class Parent
{
    public int Id { get; set; }
    public string Relation { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<Child> Children { get; set; } = new List<Child>();
}