using Microsoft.AspNetCore.Identity;

namespace Soleil.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Parent? ParentProfile { get; set; }
    public virtual Doctor? DoctorProfile { get; set; }
}