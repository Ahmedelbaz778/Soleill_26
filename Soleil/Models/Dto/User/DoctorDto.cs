namespace Soleil.Models.DTOs.Doctor;

public class DoctorDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string City { get; set; } = string.Empty;
    public string? ClinicPhone { get; set; }
    public string? WorkingHours { get; set; }
    public string? ProfileImage { get; set; }
}