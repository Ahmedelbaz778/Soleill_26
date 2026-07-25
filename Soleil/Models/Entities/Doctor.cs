namespace Soleil.Models.Entities;

public class Doctor
{
    public int Id { get; set; }
    public string Education { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? Building { get; set; }
    public string? ClinicPhone { get; set; }
    public string? WorkingHours { get; set; }
    public string? CertificateImage { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsVerified { get; set; } = false;
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}