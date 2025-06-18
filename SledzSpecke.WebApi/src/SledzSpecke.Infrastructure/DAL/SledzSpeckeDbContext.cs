using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Outbox;
using SledzSpecke.Infrastructure.DAL.Configurations;
using SledzSpecke.Infrastructure.Outbox.Data;
using SledzSpecke.Infrastructure.Persistence.EntityTypeConfigurations;
using System.Linq;

namespace SledzSpecke.Infrastructure.DAL;

public sealed class SledzSpeckeDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<MedicalShift> MedicalShifts => Set<MedicalShift>();
    public DbSet<ProcedureBase> Procedures => Set<ProcedureBase>();
    public DbSet<ProcedureOldSmk> ProceduresOldSmk => Set<ProcedureOldSmk>();
    public DbSet<ProcedureNewSmk> ProceduresNewSmk => Set<ProcedureNewSmk>();
    public DbSet<Internship> Internships => Set<Internship>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<Recognition> Recognitions => Set<Recognition>();
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<SelfEducation> SelfEducations => Set<SelfEducation>();
    public DbSet<EducationalActivity> EducationalActivities => Set<EducationalActivity>();
    public DbSet<FileMetadata> FileMetadata => Set<FileMetadata>();
    public DbSet<AdditionalSelfEducationDays> AdditionalSelfEducationDays => Set<AdditionalSelfEducationDays>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public SledzSpeckeDbContext(DbContextOptions<SledzSpeckeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicitly ignore all value objects to prevent them from being treated as entities
        var valueObjectTypes = typeof(Core.ValueObjects.ModuleId).Assembly
            .GetTypes()
            .Where(t => t.Namespace == "SledzSpecke.Core.ValueObjects" && !t.IsAbstract && !t.IsInterface)
            .ToList();
            
        foreach (var valueObjectType in valueObjectTypes)
        {
            modelBuilder.Ignore(valueObjectType);
        }
        
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SpecializationConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleConfiguration());
        modelBuilder.ApplyConfiguration(new MedicalShiftConfiguration());
        modelBuilder.ApplyConfiguration(new ProcedureBaseConfiguration());
        modelBuilder.ApplyConfiguration(new ProcedureOldSmkConfiguration());
        modelBuilder.ApplyConfiguration(new ProcedureNewSmkConfiguration());
        modelBuilder.ApplyConfiguration(new InternshipConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new AbsenceConfiguration());
        modelBuilder.ApplyConfiguration(new RecognitionConfiguration());
        modelBuilder.ApplyConfiguration(new PublicationConfiguration());
        modelBuilder.ApplyConfiguration(new SelfEducationConfiguration());
        modelBuilder.ApplyConfiguration(new EducationalActivityConfiguration());
        modelBuilder.ApplyConfiguration(new FileMetadataConfiguration());
        modelBuilder.ApplyConfiguration(new AdditionalSelfEducationDaysConfiguration());
        
        // Add Outbox configuration
        modelBuilder.ApplyConfiguration(new Configurations.OutboxMessageConfiguration());
    }
}