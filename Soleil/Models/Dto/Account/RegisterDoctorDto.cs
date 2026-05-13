using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Soleil.Models.DTOs.Account;

public class RegisterDoctorDto
{
    [Required(ErrorMessage = "الاسم الأول مطلوب")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم الأخير مطلوب")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "المؤهل العلمي مطلوب")]
    public string Education { get; set; } = string.Empty;

    [Required(ErrorMessage = "سنوات الخبرة مطلوبة")]
    public int ExperienceYears { get; set; }

    [Required(ErrorMessage = "الرقم القومي مطلوب")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "المدينة مطلوبة")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الشارع مطلوب")]
    public string Street { get; set; } = string.Empty;

    public string? Building { get; set; }

    public string? ClinicPhone { get; set; }
    public string? WorkingHours { get; set; }

    // لاستلام ملف الصورة الفعلي من الموبايل
    public IFormFile? CertificateImage { get; set; }
}