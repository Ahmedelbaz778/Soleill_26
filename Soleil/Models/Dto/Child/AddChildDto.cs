using System.ComponentModel.DataAnnotations;

namespace Soleil.Models.Dto.Child;

public class AddChildDto
{
    [Required(ErrorMessage = "اسم الطفل مطلوب")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "نوع الطفل مطلوب")]
    public string Gender { get; set; } = string.Empty; // Male or Female

}