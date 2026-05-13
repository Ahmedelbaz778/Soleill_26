using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTO.Scan.EyeScan;
using Soleil.Models.DTO.Scan.EyeScan;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Repositories;

public class EyeScanRepository : IEyeScanRepository
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public EyeScanRepository(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<EyeScanResultDto> AnalyzeAsync(EyeScanRequestDto dto)
    {
        // 1. تحويل الإحداثيات → CSV
        var csvContent = ConvertToCsv(dto.ScanPath);
        var csvBytes = Encoding.UTF8.GetBytes(csvContent);

        // 2. بعت CSV للـ AI Model
        using var formData = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(csvBytes);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        formData.Add(fileContent, "file", "scanpath.csv");

        var aiUrl = $"https://elamit1911-asd-api.hf.space/predict?child_id={dto.ChildId}&notes={dto.Notes ?? "none"}";
        var response = await _httpClient.PostAsync(aiUrl, formData);

        if (!response.IsSuccessStatusCode)
            throw new Exception("فشل في الاتصال بالـ AI Model");

        // 3. قراءة النتيجة
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var aiResult = JsonSerializer.Deserialize<AiResponseDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        if (aiResult == null)
            throw new Exception("فشل في قراءة نتيجة الـ AI");

        // 4. حفظ النتيجة في DB
        var eyeScan = new EyeScanTest
        {
            ChildId = dto.ChildId,
            AsdProbability = aiResult.AsdProbability,
            TdProbability = aiResult.TdProbability,
            Result = aiResult.Result,
            Confidence = aiResult.Confidence,
            Recommendation = aiResult.Recommendation,
            PointsAnalyzed = aiResult.PointsAnalyzed,
            Decision = GetDecision(aiResult.AsdProbability * 100),
            Date = DateTime.UtcNow
        };

        await _context.EyeScanTests.AddAsync(eyeScan);
        await _context.SaveChangesAsync();

        // 5. رجوع النتيجة
        return new EyeScanResultDto
        {
            AsdProbability = aiResult.AsdProbability,
            TdProbability = aiResult.TdProbability,
            Result = aiResult.Result,
            Confidence = aiResult.Confidence,
            Recommendation = aiResult.Recommendation,
            PointsAnalyzed = aiResult.PointsAnalyzed,
            Decision = eyeScan.Decision
        };
    }

    // تحويل الإحداثيات لـ CSV
    private string ConvertToCsv(List<ScanPointDto> scanPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("idx,x,y,duration");
        foreach (var point in scanPath)
        {
            sb.AppendLine($"{point.Idx},{point.X},{point.Y},{point.Duration}");
        }
        return sb.ToString();
    }

    // القرار بناءً على الـ BRD
    private string GetDecision(double percentage)
    {
        if (percentage < 30)
            return "لا توجد مؤشرات مقلقة";
        if (percentage <= 80)
            return "يحتاج استبيان";
        return "يرجى مراجعة طبيب مختص";
    }
}