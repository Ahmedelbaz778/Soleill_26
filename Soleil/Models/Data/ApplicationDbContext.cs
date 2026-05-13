using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soleil.Models.Entities; // استدعاء الكلاسات اللي أنشأناها

namespace Soleil.Models.Data; // المسار الجديد اللي اخترته

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // جداول المستخدمين والبروفايلات
    public DbSet<Parent> Parents { get; set; }
    public DbSet<Doctor> Doctors { get; set; }

    // جداول الأطفال والتقييم
    public DbSet<Child> Children { get; set; }
    public DbSet<EyeScanTest> EyeScanTests { get; set; }
    public DbSet<QuestionnaireField> QuestionnaireFields { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionnaireResult> QuestionnaireResults { get; set; }

    // جداول الألعاب والتقدم
    public DbSet<Game> Games { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<ChildProgress> ChildProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // ضروري جداً لتشغيل جداول الـ Identity الافتراضية

        // 1. علاقة الأب بالمستخدم (1-to-1)
        builder.Entity<Parent>()
            .HasOne(p => p.User)
            .WithOne(u => u.ParentProfile)
            .HasForeignKey<Parent>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 2. علاقة الدكتور بالمستخدم (1-to-1)
        builder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne(u => u.DoctorProfile)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. علاقة الطفل بولي الأمر (1-to-Many)
        builder.Entity<Child>()
            .HasOne(c => c.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. ربط اللعبة بمجال التقييم (Restrict عشان الداتا متضيعش)
        builder.Entity<Game>()
            .HasOne(g => g.Field)
            .WithMany(f => f.Games)
            .HasForeignKey(g => g.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. التأكد من عدم تكرار الرقم القومي للطبيب
        builder.Entity<Doctor>()
            .HasIndex(d => d.NationalId)
            .IsUnique();

        // 6. ربط سجل التقدم بالطفل (1-to-1)
        builder.Entity<ChildProgress>()
            .HasOne(cp => cp.Child)
            .WithOne(c => c.ChildProgress)
            .HasForeignKey<ChildProgress>(cp => cp.ChildId);
    }
}