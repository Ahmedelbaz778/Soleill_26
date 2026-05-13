using System.ComponentModel.DataAnnotations;

namespace Soleil.Models.DTOs.Account;

public class RegisterParentDto
{
    [Required(ErrorMessage = "الاسم الأول مطلوب")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم الأخير مطلوب")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(6, ErrorMessage = "كلمة المرور لا تقل عن 6 أحرف")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "صلة القرابة مطلوبة")]
    public string Relation { get; set; } = string.Empty;
}