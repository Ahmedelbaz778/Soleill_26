using Soleil.Models.DTO.Scan.EyeScan;

namespace Soleil.Infrastructure.Interfaces;

public interface IEyeScanRepository
{
    Task<EyeScanResultDto> AnalyzeAsync(EyeScanRequestDto dto);
}