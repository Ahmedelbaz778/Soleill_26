namespace Soleil.Models.DTOs.Doctor;

public class UpdateDoctorDto
{
    public string? Education { get; set; }
    public int? ExperienceYears { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    public string? ClinicPhone { get; set; }
    public string? WorkingHours { get; set; }
    public IFormFile? ProfileImage { get; set; }
}