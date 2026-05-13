namespace Soleil.Models.DTOs.Assessment;

public class AssessmentReportDto
{
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public double Score { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}