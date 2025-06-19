using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SledzSpecke.Application.SpecializationTemplates.Services;
using SledzSpecke.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;

class TestImportService
{
    static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<SledzSpeckeDbContext>(options =>
                    options.UseNpgsql("Host=localhost;Database=sledzspecke_db;Username=sledzspecke_user;Password=Sledz2024!;"));
                
                services.AddScoped<ISpecializationTemplateRepository, SledzSpecke.Infrastructure.Repositories.SpecializationTemplateRepository>();
                services.AddScoped<ISpecializationTemplateImportService, SpecializationTemplateImportService>();
                services.AddLogging();
            })
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var importService = scope.ServiceProvider.GetRequiredService<ISpecializationTemplateImportService>();
            
            Console.WriteLine("Testing Specialization Template Import Service");
            Console.WriteLine("===========================================");
            
            // Test 1: Get all templates
            Console.WriteLine("\n1. Getting all templates from database:");
            var allTemplatesResult = await importService.GetAllTemplatesAsync();
            if (allTemplatesResult.IsSuccess)
            {
                Console.WriteLine($"   Found {allTemplatesResult.Value.Count} templates");
                foreach (var template in allTemplatesResult.Value)
                {
                    Console.WriteLine($"   - {template.Code} ({template.Version}): {template.Name}");
                }
            }
            else
            {
                Console.WriteLine($"   Error: {allTemplatesResult.Error}");
            }
            
            // Test 2: Import from directory
            Console.WriteLine("\n2. Importing templates from JSON files:");
            var importResult = await importService.ImportFromDirectoryAsync("");
            if (importResult.IsSuccess)
            {
                Console.WriteLine($"   Successfully imported {importResult.Value.Count} templates");
            }
            else
            {
                Console.WriteLine($"   Error: {importResult.Error}");
            }
            
            // Test 3: Get all templates again
            Console.WriteLine("\n3. Checking templates after import:");
            allTemplatesResult = await importService.GetAllTemplatesAsync();
            if (allTemplatesResult.IsSuccess)
            {
                Console.WriteLine($"   Found {allTemplatesResult.Value.Count} templates");
                foreach (var template in allTemplatesResult.Value)
                {
                    Console.WriteLine($"   - {template.Code} ({template.Version}): {template.Name}");
                }
            }
            
            Console.WriteLine("\nTest completed!");
        }
    }
}