namespace Soleil.Models.Dto.Progress;

public class ChildReportDto
{
    public string ChildName { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public EyeScanReportDto? EyeScan { get; set; }
    public List<QuestionnaireReportDto> Questionnaire { get; set; } = new();
    public string? WeakestField { get; set; }
    public string? SuggestedGame { get; set; }
}

public class EyeScanReportDto
{
    public double AsdProbability { get; set; }
    public double TdProbability { get; set; }
    public string Result { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Decision { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class QuestionnaireReportDto
{
    public string FieldName { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Status { get; set; } = string.Empty;
}