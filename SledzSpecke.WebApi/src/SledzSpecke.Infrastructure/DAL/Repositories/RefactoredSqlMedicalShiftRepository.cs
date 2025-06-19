using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored MedicalShift repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlMedicalShiftRepository : BaseRepository<MedicalShift>, IMedicalShiftRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInternshipRepository _internshipRepository;

    public RefactoredSqlMedicalShiftRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork,
        IInternshipRepository internshipRepository) : base(context)
    {
        _unitOfWork = unitOfWork;
        _internshipRepository = internshipRepository;
    }

    public async Task<MedicalShift?> GetByIdAsync(int id)
    {
        // Need to convert int to MedicalShiftId for EF Core to properly find the entity
        var medicalShiftId = new MedicalShiftId(id);
        return await Context.MedicalShifts.FirstOrDefaultAsync(ms => ms.Id == medicalShiftId);
    }

    public async Task<IEnumerable<MedicalShift>> GetByInternshipIdAsync(int internshipId)
    {
        var specification = new MedicalShiftByInternshipSpecification(new InternshipId(internshipId));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserIdAsync(int userId)
    {
        // First get all internships for the user
        var internshipIds = await GetInternshipIdsForUserAsync(userId);
        
        // Then use specification to get medical shifts
        var specification = new MedicalShiftByInternshipIdsSpecification(internshipIds);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserAsync(UserId userId)
    {
        return await GetByUserIdAsync(userId.Value);
    }

    public async Task<IEnumerable<MedicalShift>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
    {
        // Get internship IDs for user
        var internshipIds = await GetInternshipIdsForUserAsync(userId);
        
        // Combine specifications
        var specification = new MedicalShiftByInternshipIdsSpecification(internshipIds)
            .And(new MedicalShiftByDateRangeSpecification(startDate, endDate));
            
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<MedicalShift>> GetAllAsync()
    {
        return await GetAllAsync(default);
    }

    public async Task<int> GetTotalHoursAsync(int internshipId)
    {
        var specification = new MedicalShiftByInternshipSpecification(new InternshipId(internshipId));
        var shifts = await GetBySpecificationAsync(specification);
        return shifts.Sum(s => s.Hours + (s.Minutes / 60));
    }

    public async Task<int> AddAsync(MedicalShift shift)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (shift.Id.Value == 0)
        {
            await GenerateIdForEntity(shift);
        }

        await AddAsync(shift, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
        return shift.Id.Value;
    }

    public async Task UpdateAsync(MedicalShift shift)
    {
        Update(shift);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<MedicalShift>> GetByUserIdAndDateRangeAsync(UserId userId, DateTime startDate, DateTime endDate)
    {
        // Use GetByDateRangeAsync and filter by user
        // This is a temporary implementation until we have proper specifications
        var shifts = await GetByDateRangeAsync(startDate, endDate, userId.Value);
        return shifts;
    }

    public async Task DeleteAsync(int id)
    {
        var shift = await GetByIdAsync(id);
        if (shift != null)
        {
            Remove(shift);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<MedicalShift>> GetPendingShiftsForInternshipAsync(InternshipId internshipId)
    {
        var specification = MedicalShiftSpecificationExtensions.GetPendingShiftsForInternship(internshipId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<MedicalShift>> GetApprovedShiftsForMonthAsync(int internshipId, int year, int month)
    {
        var specification = MedicalShiftSpecificationExtensions.GetApprovedShiftsForMonth(internshipId, year, month);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> HasConflictingShiftAsync(InternshipId internshipId, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        var specification = new MedicalShiftByInternshipSpecification(internshipId)
            .And(new MedicalShiftByDateRangeSpecification(date.Date, date.Date.AddDays(1)));
            
        var shiftsOnDate = await GetBySpecificationAsync(specification);
        
        // Check for time conflicts
        return shiftsOnDate.Any(shift =>
        {
            var shiftStart = shift.Date.Date.Add(TimeSpan.FromHours(shift.Hours)); // Simplified
            var shiftEnd = shiftStart.AddMinutes(shift.Minutes);
            var newStart = date.Date.Add(startTime);
            var newEnd = date.Date.Add(endTime);
            
            return (newStart < shiftEnd && newEnd > shiftStart); // Overlap check
        });
    }

    // Private helper methods
    private async Task<List<InternshipId>> GetInternshipIdsForUserAsync(int userId)
    {
        // TODO: User-Specialization relationship needs to be redesigned
        // The User entity no longer has SpecializationId property
        // This query needs to be refactored once the new relationship model is established
        // This could be refactored to use specifications on Internship repository
        /*
        var internships = await Context.Internships
            .Join(Context.Specializations,
                i => i.SpecializationId,
                s => s.Id,
                (i, s) => new { Internship = i, Specialization = s })
            .Join(Context.Users,
                x => x.Specialization.Id,
                u => u.SpecializationId,
                (x, u) => new { x.Internship, User = u })
            .Where(x => x.User.Id.Value == userId)
            .Select(x => x.Internship.Id)
            .ToListAsync();

        return internships.Select(id => new InternshipId(id)).ToList();
        */
        return new List<InternshipId>(); // Temporary empty list
    }

    private async Task GenerateIdForEntity(MedicalShift shift)
    {
        // This should ideally be in a separate ID generation service
        var connection = Context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"MedicalShifts\"";
        var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

        var newId = new MedicalShiftId(maxId + 1);

        // Use reflection to set the ID since it's private
        var idProperty = shift.GetType().GetProperty("Id");
        idProperty?.SetValue(shift, newId);

        await connection.CloseAsync();
    }
}