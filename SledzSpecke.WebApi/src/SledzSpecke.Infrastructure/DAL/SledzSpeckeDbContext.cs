using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Infrastructure.DAL.Configurations;

namespace SledzSpecke.Infrastructure.DAL;

public sealed class SledzSpeckeDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<MedicalShift> MedicalShifts => Set<MedicalShift>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<Internship> Internships => Set<Internship>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<Recognition> Recognitions => Set<Recognition>();
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<SelfEducation> SelfEducations => Set<SelfEducation>();

    public SledzSpeckeDbContext(DbContextOptions<SledzSpeckeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SpecializationConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleConfiguration());
        modelBuilder.ApplyConfiguration(new MedicalShiftConfiguration());
        modelBuilder.ApplyConfiguration(new ProcedureConfiguration());
        modelBuilder.ApplyConfiguration(new InternshipConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new AbsenceConfiguration());
        modelBuilder.ApplyConfiguration(new RecognitionConfiguration());
        modelBuilder.ApplyConfiguration(new PublicationConfiguration());
        modelBuilder.ApplyConfiguration(new SelfEducationConfiguration());
    }
}