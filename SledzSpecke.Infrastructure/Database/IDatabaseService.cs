// <copyright file="IDatabaseService.cs" company="SledzSpecke">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Infrastructure.Database
{
    /// <summary>
    /// Interface for database service providing access to application data.
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InitAsync();

        /// <summary>
        /// Gets all objects of specified type.
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve.</typeparam>
        /// <returns>List of objects of specified type.</returns>
        Task<List<T>> GetAllAsync<T>()
            where T : new();

        /// <summary>
        /// Gets an object by its ID.
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve.</typeparam>
        /// <param name="id">ID of the object.</param>
        /// <returns>Object with specified ID or null if not found.</returns>
        Task<T?> GetByIdAsync<T>(int id)
            where T : class, new();

        /// <summary>
        /// Saves an object to the database.
        /// </summary>
        /// <typeparam name="T">Type of object to save.</typeparam>
        /// <param name="item">Object to save.</param>
        /// <returns>Number of rows changed or object ID.</returns>
        Task<int> SaveAsync<T>(T item)
            where T : new();

        /// <summary>
        /// Deletes an object from the database.
        /// </summary>
        /// <typeparam name="T">Type of object to delete.</typeparam>
        /// <param name="item">Object to delete.</param>
        /// <returns>Number of rows deleted.</returns>
        Task<int> DeleteAsync<T>(T item)
            where T : new();

        /// <summary>
        /// Executes a SQL query and returns a list of objects.
        /// </summary>
        /// <typeparam name="T">Type of object to return.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">Query arguments.</param>
        /// <returns>List of objects resulting from the query.</returns>
        Task<List<T>> QueryAsync<T>(string query, params object[] args)
            where T : new();

        /// <summary>
        /// Executes a SQL query without returning results.
        /// </summary>
        /// <param name="query">SQL query.</param>
        /// <param name="args">Query arguments.</param>
        /// <returns>Number of rows affected.</returns>
        Task<int> ExecuteAsync(string query, params object[] args);

        /// <summary>
        /// Gets medical procedures for a specific internship.
        /// </summary>
        /// <param name="internshipId">Internship ID.</param>
        /// <returns>List of medical procedures.</returns>
        Task<List<MedicalProcedure>> GetProceduresForInternshipAsync(int internshipId);

        /// <summary>
        /// Gets entries for a specific medical procedure.
        /// </summary>
        /// <param name="procedureId">Procedure ID.</param>
        /// <returns>List of procedure entries.</returns>
        Task<List<ProcedureEntry>> GetEntriesForProcedureAsync(int procedureId);

        /// <summary>
        /// Gets courses for a specific module.
        /// </summary>
        /// <param name="moduleType">Module type.</param>
        /// <returns>List of courses.</returns>
        Task<List<Course>> GetCoursesForModuleAsync(ModuleType moduleType);

        /// <summary>
        /// Gets internships for a specific module.
        /// </summary>
        /// <param name="moduleType">Module type.</param>
        /// <returns>List of internships.</returns>
        Task<List<Internship>> GetInternshipsForModuleAsync(ModuleType moduleType);

        /// <summary>
        /// Gets user settings.
        /// </summary>
        /// <returns>User settings.</returns>
        Task<UserSettings> GetUserSettingsAsync();

        /// <summary>
        /// Saves user settings.
        /// </summary>
        /// <param name="settings">User settings to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveUserSettingsAsync(UserSettings settings);

        /// <summary>
        /// Gets the current specialization.
        /// </summary>
        /// <returns>Current specialization or null if not found.</returns>
        Task<Specialization?> GetCurrentSpecializationAsync();

        /// <summary>
        /// Deletes all data from the database.
        /// </summary>
        /// <returns>True if operation was successful, otherwise False.</returns>
        Task<bool> DeleteAllDataAsync();

        /// <summary>
        /// Checks if the specialization already has template data loaded.
        /// </summary>
        /// <param name="specializationTypeId">Specialization type ID.</param>
        /// <returns>True if template data exists, otherwise False.</returns>
        Task<bool> HasSpecializationTemplateDataAsync(int specializationTypeId);

        /// <summary>
        /// Initializes template data for a specific specialization type.
        /// </summary>
        /// <param name="specializationTypeId">Specialization type ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InitializeSpecializationTemplateDataAsync(int specializationTypeId);

        /// <summary>
        /// Inserts a new object into the database.
        /// </summary>
        /// <typeparam name="T">Type of object to insert.</typeparam>
        /// <param name="item">Object to insert.</param>
        /// <returns>ID of the inserted object or number of rows changed.</returns>
        Task<int> InsertAsync<T>(T item)
            where T : new();

        /// <summary>
        /// Updates an existing object in the database.
        /// </summary>
        /// <typeparam name="T">Type of object to update.</typeparam>
        /// <param name="item">Object to update.</param>
        /// <returns>Number of rows changed.</returns>
        Task<int> UpdateAsync<T>(T item)
            where T : new();
    }
}