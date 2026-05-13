using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Account;
using Soleil.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Soleil.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager; // ✅ محتاجه للـ Roles
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AccountRepository(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _config = config;
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        => await _userManager.CreateAsync(user, password);

    public async Task AddParentAsync(Parent parent)
        => await _context.Parents.AddAsync(parent);

    public async Task AddDoctorAsync(Doctor doctor)
        => await _context.Doctors.AddAsync(doctor);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public async Task<ApplicationUser?> ValidateUserAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return user;
        return null;
    }

    // ✅ تعيين الـ Role للمستخدم
    public async Task AssignRoleAsync(ApplicationUser user, string role)
    {
        // لو الـ Role مش موجود ينشئه
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userManager.AddToRoleAsync(user, role);
    }

    // ✅ جيب الـ Role بتاع المستخدم
    public async Task<string?> GetUserRoleAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }

    public string GenerateJwtToken(ApplicationUser user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FullName", $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}