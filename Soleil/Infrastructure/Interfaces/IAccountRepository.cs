using Microsoft.AspNetCore.Identity;
using Soleil.Models.DTOs.Account;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Interfaces;

public interface IAccountRepository
{
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task AddParentAsync(Parent parent);
    Task AddDoctorAsync(Doctor doctor);
    Task AssignRoleAsync(ApplicationUser user, string role);
    Task<string?> GetUserRoleAsync(ApplicationUser user);

    Task SaveChangesAsync();
    Task<ApplicationUser?> ValidateUserAsync(LoginDto loginDto);
    string GenerateJwtToken(ApplicationUser user);

    
    Task<string> ForgotPasswordAsync(string email);
}