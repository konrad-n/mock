// <copyright file="DatabaseService.cs" company="Konrad Niedźwiedzki">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database.Initialization;
using SledzSpecke.Infrastructure.Services;
using SQLite;

namespace SledzSpecke.Infrastructure.Database
{
    /// <summary>
    /// Implementation of the database service providing access to application data.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<DatabaseService> logger;
        private readonly IFileSystemService fileSystemService;
        private readonly SemaphoreSlim initLock = new SemaphoreSlim(1, 1);

        private SQLiteAsyncConnection? database;
        private bool isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        /// <param name="fileSystemService">File system service used to get database path.</param>
        /// <param name="logger">Logger for recording database operation events.</param>
        public DatabaseService(IFileSystemService fileSystemService, ILogger<DatabaseService> logger)
        {
            this.fileSystemService = fileSystemService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            // Use a semaphore to prevent concurrent initialization from multiple threads
            await this.initLock.WaitAsync();

            try
            {
                if (this.isInitialized)
                {
                    return;
                }

                var databasePath = Path.Combine(this.fileSystemService.GetAppDataDirectory(), "SledzSpecke.db3");
                this.logger.LogInformation("Initializing database at {Path}", databasePath);

                // Check if directory exists
                var directory = Path.GetDirectoryName(databasePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    this.logger.LogInformation("Created database directory at {Path}", directory);
                }

                // Create database connection with additional options
                var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;
                this.database = new SQLiteAsyncConnection(databasePath, flags);

                // Create tables for all models
                await this.database.CreateTableAsync<User>().ConfigureAwait(false);
                await this.database.CreateTableAsync<SpecializationType>().ConfigureAwait(false);
                await this.database.CreateTableAsync<Specialization>().ConfigureAwait(false);
                await this.database.CreateTableAsync<Course>().ConfigureAwait(false);
                await this.database.CreateTableAsync<Internship>().ConfigureAwait(false);
                await this.database.CreateTableAsync<MedicalProcedure>().ConfigureAwait(false);
                await this.database.CreateTableAsync<ProcedureEntry>().ConfigureAwait(false);
                await this.database.CreateTableAsync<DutyShift>().ConfigureAwait(false);
                await this.database.CreateTableAsync<SelfEducation>().ConfigureAwait(false);
                await this.database.CreateTableAsync<UserSettings>().ConfigureAwait(false);
                await this.database.CreateTableAsync<Absence>().ConfigureAwait(false);

                this.isInitialized = true;
                this.logger.LogInformation("Database initialized successfully at {Path}", databasePath);
            }
            catch (Exception)
            {
                // Simply rethrow the exception without logging it here
                // to avoid duplicate logging in the call stack
                throw;
            }
            finally
            {
                this.initLock.Release();
            }
        }

        /// <inheritdoc/>
        public async Task<List<T>> GetAllAsync<T>()
            where T : new()
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.Table<T>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting all items of type {Type}", typeof(T).Name);
                return new List<T>();
            }
        }

        /// <inheritdoc/>
        public async Task<T?> GetByIdAsync<T>(int id)
            where T : class, new()
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.FindAsync<T>(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting item of type {Type} with ID {Id}", typeof(T).Name, id);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<int> SaveAsync<T>(T item)
            where T : new()
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            return await this.database.InsertOrReplaceAsync(item).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<int> DeleteAsync<T>(T item)
            where T : new()
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            return await this.database.DeleteAsync(item).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<T>> QueryAsync<T>(string query, params object[] args)
            where T : new()
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.QueryAsync<T>(query, args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error executing query {Query} for type {Type}", query, typeof(T).Name);
                return new List<T>();
            }
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            return await this.database.ExecuteAsync(query, args).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<MedicalProcedure>> GetProceduresForInternshipAsync(int internshipId)
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.Table<MedicalProcedure>()
                    .Where(p => p.InternshipId == internshipId)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting procedures for internship {InternshipId}", internshipId);
                return new List<MedicalProcedure>();
            }
        }

        /// <inheritdoc/>
        public async Task<List<ProcedureEntry>> GetEntriesForProcedureAsync(int procedureId)
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.Table<ProcedureEntry>()
                    .Where(e => e.ProcedureId == procedureId)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting entries for procedure {ProcedureId}", procedureId);
                return new List<ProcedureEntry>();
            }
        }

        /// <inheritdoc/>
        public async Task<List<Course>> GetCoursesForModuleAsync(ModuleType moduleType)
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.Table<Course>()
                    .Where(c => c.Module == moduleType)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting courses for module {ModuleType}", moduleType);
                return new List<Course>();
            }
        }

        /// <inheritdoc/>
        public async Task<List<Internship>> GetInternshipsForModuleAsync(ModuleType moduleType)
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                return await this.database.Table<Internship>()
                    .Where(i => i.Module == moduleType)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting internships for module {ModuleType}", moduleType);
                return new List<Internship>();
            }
        }

        /// <inheritdoc/>
        public async Task<UserSettings> GetUserSettingsAsync()
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                this.logger.LogDebug("Getting user settings");
                var settings = await this.database.Table<UserSettings>().FirstOrDefaultAsync().ConfigureAwait(false);
                this.logger.LogDebug("User settings retrieved: {Settings}", settings != null ? "Found" : "Not found");
                return settings ?? new UserSettings();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting user settings");
                return new UserSettings();
            }
        }

        /// <inheritdoc/>
        public async Task SaveUserSettingsAsync(UserSettings settings)
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            await this.database.InsertOrReplaceAsync(settings).ConfigureAwait(false);
            this.logger.LogInformation("User settings saved successfully");
        }

        /// <inheritdoc/>
        public async Task<Specialization?> GetCurrentSpecializationAsync()
        {
            await this.EnsureInitializedAsync();
            try
            {
                var userSettings = await this.GetUserSettingsAsync().ConfigureAwait(false);

                if (userSettings.CurrentSpecializationId == 0)
                {
                    this.logger.LogInformation("No current specialization ID found in settings");
                    return null;
                }

                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                var specialization = await this.database.FindAsync<Specialization>(userSettings.CurrentSpecializationId).ConfigureAwait(false);
                this.logger.LogDebug("Current specialization retrieved: {Specialization}", specialization != null ? specialization.Name : "Not found");
                return specialization;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting current specialization");
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllDataAsync()
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            await this.database.DeleteAllAsync<Course>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<Internship>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<MedicalProcedure>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<ProcedureEntry>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<DutyShift>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<SelfEducation>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<Specialization>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<User>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<UserSettings>().ConfigureAwait(false);
            await this.database.DeleteAllAsync<Absence>().ConfigureAwait(false);
            this.logger.LogInformation("All data deleted from the database");
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> HasSpecializationTemplateDataAsync(int specializationTypeId)
        {
            await this.EnsureInitializedAsync();
            try
            {
                if (this.database == null)
                {
                    throw new InvalidOperationException("Database is not initialized");
                }

                // Check if specialization of given type exists
                var specialization = await this.database.Table<Specialization>()
                    .Where(s => s.SpecializationTypeId == specializationTypeId)
                    .FirstOrDefaultAsync();

                if (specialization == null)
                {
                    return false;
                }

                // Check if courses exist for this specialization
                var courses = await this.database.Table<Course>()
                    .Where(c => c.SpecializationId == specialization.Id)
                    .CountAsync();

                return courses > 0;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error checking specialization template data");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task InitializeSpecializationTemplateDataAsync(int specializationTypeId)
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            // Check if template data already exists
            if (await this.HasSpecializationTemplateDataAsync(specializationTypeId))
            {
                this.logger.LogInformation("Template data already exists for specialization type {SpecializationTypeId}", specializationTypeId);
                return;
            }

            this.logger.LogInformation("Initializing template data for specialization type {SpecializationTypeId}", specializationTypeId);

            // For now, we'll use hematology data - add support for other specializations in the future
            var templateData = DataSeeder.SeedHematologySpecialization();
            templateData.SpecializationTypeId = specializationTypeId;

            // Save specialization
            await this.SaveAsync(templateData);

            // Save courses
            foreach (var course in templateData.RequiredCourses)
            {
                course.SpecializationId = templateData.Id;
                await this.SaveAsync(course);
            }

            // Save internships
            foreach (var internship in templateData.RequiredInternships)
            {
                internship.SpecializationId = templateData.Id;
                await this.SaveAsync(internship);
            }

            // Save procedures
            foreach (var procedure in templateData.RequiredProcedures)
            {
                procedure.SpecializationId = templateData.Id;
                await this.SaveAsync(procedure);
            }

            this.logger.LogInformation("Template data initialized successfully for specialization type {SpecializationTypeId}", specializationTypeId);
        }

        /// <inheritdoc/>
        public async Task<int> InsertAsync<T>(T item)
            where T : new()
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            this.logger.LogDebug("Inserting new item of type {Type}", typeof(T).Name);
            return await this.database.InsertAsync(item).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateAsync<T>(T item)
            where T : new()
        {
            await this.EnsureInitializedAsync();

            if (this.database == null)
            {
                throw new InvalidOperationException("Database is not initialized");
            }

            this.logger.LogDebug("Updating item of type {Type}", typeof(T).Name);
            return await this.database.UpdateAsync(item).ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures the database is initialized before performing operations.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task EnsureInitializedAsync()
        {
            if (!this.isInitialized)
            {
                await this.InitAsync();
            }
        }
    }
}
