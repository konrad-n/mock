namespace SledzSpecke.Infrastructure.DAL.Seeding;

public interface IDataSeeder
{
    Task SeedSpecializationTemplatesAsync();
    Task SeedBasicDataAsync();
}