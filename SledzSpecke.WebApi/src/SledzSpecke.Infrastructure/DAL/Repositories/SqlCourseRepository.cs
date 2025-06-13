using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class SqlCourseRepository : ICourseRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Course> _courses;

    public SqlCourseRepository(SledzSpeckeDbContext context)
    {
        _context = context;
        _courses = context.Courses;
    }

    public async Task<Course?> GetByIdAsync(CourseId id)
        => await _courses.SingleOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Course>> GetBySpecializationIdAsync(SpecializationId specializationId)
        => await _courses
            .Where(c => c.SpecializationId == specializationId)
            .ToListAsync();

    public async Task<IEnumerable<Course>> GetByModuleIdAsync(ModuleId moduleId)
        => await _courses
            .Where(c => c.ModuleId == moduleId)
            .ToListAsync();

    public async Task<IEnumerable<Course>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
        => await _courses
            .Where(c => c.SpecializationId == specializationId)
            .ToListAsync();

    public async Task<IEnumerable<Course>> GetByModuleAsync(ModuleId moduleId)
        => await _courses
            .Where(c => c.ModuleId == moduleId)
            .ToListAsync();

    public async Task<IEnumerable<Course>> GetByTypeAsync(CourseType courseType)
        => await _courses
            .Where(c => c.CourseType == courseType)
            .ToListAsync();

    public async Task<IEnumerable<Course>> GetPendingApprovalAsync()
        => await _courses
            .Where(c => !c.IsApproved)
            .ToListAsync();

    public async Task AddAsync(Course course)
    {
        await _courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CourseId id)
    {
        var course = await _courses.FindAsync(id.Value);
        if (course is not null)
        {
            _courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}