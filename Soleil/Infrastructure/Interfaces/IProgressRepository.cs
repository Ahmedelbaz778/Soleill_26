using Soleil.Models.Dto.Progress;


namespace Soleil.Infrastructure.Interfaces;

public interface IProgressRepository
{
    Task<ChildReportDto?> GetLatestReportAsync(int childId);
}