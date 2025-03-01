using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database
{
    public interface IDatabaseService
    {
        Task InitAsync();
        Task<List<T>> GetAllAsync<T>() where T : new();
        Task<T> GetByIdAsync<T>(int id) where T : class, new();
        Task<int> SaveAsync<T>(T item) where T : new();
        Task<int> DeleteAsync<T>(T item) where T : new();
        Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new();
        Task<int> ExecuteAsync(string query, params object[] args);
        Task<List<MedicalProcedure>> GetProceduresForInternshipAsync(int internshipId);
        Task<List<ProcedureEntry>> GetEntriesForProcedureAsync(int procedureId);
        Task<List<Course>> GetCoursesForModuleAsync(ModuleType moduleType);
        Task<List<Internship>> GetInternshipsForModuleAsync(ModuleType moduleType);
        Task<UserSettings> GetUserSettingsAsync();
        Task SaveUserSettingsAsync(UserSettings settings);
        Task<Specialization> GetCurrentSpecializationAsync();
        Task<bool> DeleteAllDataAsync();
        Task<bool> HasSpecializationTemplateDataAsync(int specializationTypeId);
        Task InitializeSpecializationTemplateDataAsync(int specializationTypeId);
        Task<int> InsertAsync<T>(T item) where T : new();
        Task<int> UpdateAsync<T>(T item) where T : new();
    }
}