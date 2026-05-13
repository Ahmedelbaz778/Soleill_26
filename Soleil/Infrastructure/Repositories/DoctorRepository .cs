using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsVerified == true) 
            .Include(d => d.User)
            .ToListAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}