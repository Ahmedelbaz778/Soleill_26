using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTO.Scan.EyeScan;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Repositories;

public class EyeScanRepository : IEyeScanRepository
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private const string AiApiUrl = "https://ayat33-asd-screening-api.hf.space/predict";

    // استخدام JsonSerializerOptions ثابتة كـ static لمنع إعادة توليدها مع كل Request تحسيناً للأداء
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public EyeScanRepository(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<EyeScanResultDto> AnalyzeAsync(EyeScanRequestDto dto)
    {
        // 1. تجهيز الـ Request Body المطابق للـ Swagger الجديد {"points": [...]}
        var requestPayload = new
        {
            points = dto.ScanPath.Select(p => new
            {
                x = p.X,
                y = p.Y,
                duration_ms = p.DurationMs // التعديل الجديد للأتربيوت
            }).ToList()
        };

        // 2. إرسال الـ JSON مباشرة باستخدام PostAsJsonAsync (أسرع وأوفر في الذاكرة)
        var response = await _httpClient.PostAsJsonAsync(AiApiUrl, requestPayload);

        // 3. معالجة الأخطاء بشكل احترافي
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"AI Server Error: {response.StatusCode} - {errorContent}");
        }

        // 4. قراءة الـ Stream مباشرة وعمل Deserialize (أداء أفضل بكتير من قراءته كـ string أولاً)
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var aiResult = await JsonSerializer.DeserializeAsync<AiResponseDto>(responseStream, JsonOptions);

        if (aiResult == null)
            throw new InvalidOperationException("فشل في قراءة وتفسير نتيجة الـ AI المستلمة.");

        // 5. حفظ النتيجة في قاعدة البيانات
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

        // 6. إرجاع الـ DTO النهائي للـ Flutter
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

    private static string GetDecision(double percentage)
    {
        return percentage switch
        {
            < 30 => "لا توجد مؤشرات مقلقة",
            <= 80 => "يحتاج استبيان",
            _ => "يرجى مراجعة طبيب مختص"
        };
    }
}