// <copyright file="IFileSystemService.cs" company="Konrad Niedźwiedzki">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SledzSpecke.Infrastructure.Services
{
    /// <summary>
    /// Interface providing access to the file system.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Gets the path to the application data directory.
        /// </summary>
        /// <returns>Path to the application data directory.</returns>
        string GetAppDataDirectory();
    }
}