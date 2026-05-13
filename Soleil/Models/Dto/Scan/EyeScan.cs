namespace Soleil.Models.DTO.Scan.EyeScan;

// Input من Flutter
public class EyeScanRequestDto
{
    public int ChildId { get; set; }
    public string? Notes { get; set; }
    public List<ScanPointDto> ScanPath { get; set; } = new();
}

public class ScanPointDto
{
    public int Idx { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Duration { get; set; }
}

// Output للـ Flutter
public class EyeScanResultDto
{
    public double AsdProbability { get; set; }
    public double TdProbability { get; set; }
    public string Result { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public int PointsAnalyzed { get; set; }
    public string Decision { get; set; } = string.Empty;
}

// Response من الـ AI
public class AiResponseDto
{
    public double AsdProbability { get; set; }
    public double TdProbability { get; set; }
    public string Result { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public int PointsAnalyzed { get; set; }
}