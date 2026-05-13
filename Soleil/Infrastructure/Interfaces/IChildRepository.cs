using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Interfaces;

public interface IChildRepository
{
    Task AddChildAsync(Child child);
    Task<IEnumerable<Child>> GetChildrenByParentIdAsync(int parentId);
    Task<Child?> GetChildByIdAsync(int childId);
    Task SaveChangesAsync();
}