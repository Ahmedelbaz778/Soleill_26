using Microsoft.EntityFrameworkCore;
using Soleil.Models.Data;
using Soleil.Models.Entities;
using Soleil.Infrastructure.Interfaces;

namespace Soleil.Infrastructure.Repositories;

public class ChildRepository : IChildRepository
{
    private readonly ApplicationDbContext _context;

    public ChildRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddChildAsync(Child child)
    {
        await _context.Children.AddAsync(child);
    }

    public async Task<IEnumerable<Child>> GetChildrenByParentIdAsync(int parentId)
    {
        return await _context.Children
            .Where(c => c.ParentId == parentId)
            .ToListAsync();
    }

    public async Task<Child?> GetChildByIdAsync(int childId)
    {
        return await _context.Children.FindAsync(childId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}