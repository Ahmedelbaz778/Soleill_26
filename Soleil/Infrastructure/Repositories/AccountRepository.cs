using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Account;
using Soleil.Models.Entities;
using Soleil.Services; // ✅ ضفنا الـ using ده عشان يشوف الـ EmailService
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Soleil.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService; // ✅ ضفنا حقل خدمة الإيميل هنا

    public AccountRepository(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IConfiguration config,
        IEmailService emailService) // ✅ حقنا الـ EmailService هنا
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _config = config;
        _emailService = emailService; // ✅ ربطنا الخدمة
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

    public async Task AssignRoleAsync(ApplicationUser user, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userManager.AddToRoleAsync(user, role);
    }

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

    // 🆕 الميثود الجديدة لـ نسيان كلمة المرور بالمنطق المتوافق مع الـ Controller والتصميم الجديد
    public async Task<string> ForgotPasswordAsync(string email)
    {
        // 1. التأكد إن المستخدم موجود
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return "NotFound";
        }

        // 2. توليد باسوورد عشوائية مؤقتة تبدأ بـ Soleil
        string newPassword = $"Soleil@{Guid.NewGuid().ToString().Substring(0, 8)}";

        // 3. عمل Reset للباسوورد في الداتابيز فوراً
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return "Error";
        }

        // 4. إرسال الباسوورد الجديدة للإيميل بتصميم الـ HTML الجديد والاحترافي
        string subject = "Soleil App - إعادة تعيين كلمة المرور";
        string messageBody = $@"
            <div style='direction: rtl; font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; text-align: right; background-color: #f9f9f9; padding: 40px 20px; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); overflow: hidden; border: 1px solid #eee;'>
                    
                    <div style='background: linear-gradient(135deg, #4f46e5, #06b6d4); padding: 30px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600; letter-spacing: 1px;'>Soleil</h1>
                    </div>

                    <div style='padding: 30px 25px;'>
                        <h3 style='color: #1e1b4b; font-size: 20px; margin-top: 0; margin-bottom: 15px; font-weight: bold;'>مرحباً بك في تطبيق Soleil،</h3>
                        <p style='font-size: 16px; line-height: 1.6; color: #4b5563; margin-bottom: 25px;'>
                            لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك. تم إنشاء كلمة مرور مؤقتة جديدة وآمنة لك.
                        </p>

                        <div style='background-color: #f3f4f6; border-right: 4px solid #4f46e5; padding: 20px; border-radius: 6px; text-align: center; margin-bottom: 25px;'>
                            <span style='display: block; font-size: 14px; color: #6b7280; margin-bottom: 8px; font-weight: 500;'>كلمة المرور المؤقتة الجديدة</span>
                            <code style='font-size: 24px; color: #4f46e5; font-weight: bold; letter-spacing: 1px; font-family: Consolas, monospace;'>{newPassword}</code>
                        </div>

                        <div style='background-color: #fffbeb; border: 1px solid #fef3c7; border-radius: 8px; padding: 15px; margin-bottom: 20px;'>
                            <p style='margin: 0; font-size: 14px; color: #b45309; line-height: 1.5;'>
                                ⚠️ <b>تنبيه أمني مهم:</b> يرجى نسخ هذه الكلمة وتسجيل الدخول بها، ثم التوجه فوراً إلى إعدادات حسابك لتغييرها إلى كلمة مرور جديدة خاصة بك لضمان أمان بياناتك.
                            </p>
                        </div>
                    </div>

                    <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #f3f4f6;'>
                        <p style='margin: 0; font-size: 13px; color: #9ca3af;'>
                            إذا لم تكن أنت من طلب هذا التغيير، يرجى تجاهل هذا البريد الإلكتروني.
                        </p>
                        <p style='margin: 10px 0 0 0; font-size: 12px; color: #9ca3af;'>
                            © {DateTime.Now.Year} Soleil Project. All rights reserved.
                        </p>
                    </div>

                </div>
            </div>";

        try
        {
            await _emailService.SendEmailAsync(user.Email, subject, messageBody);
        }
        catch (Exception)
        {
            return "EmailFailed";
        }

        return "Success";
    }
}