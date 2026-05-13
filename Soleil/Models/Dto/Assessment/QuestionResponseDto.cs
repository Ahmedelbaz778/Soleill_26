namespace Soleil.Models.DTOs.Assessment;

public class QuestionResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
}