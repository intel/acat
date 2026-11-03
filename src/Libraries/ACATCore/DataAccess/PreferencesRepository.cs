////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesRepository.cs
//
// Repository for XML-serializable preferences objects.
// Delegates to the existing XmlUtils helper for file I/O.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Repository for XML-based preferences entities.
    /// Uses <see cref="XmlUtils"/> for serialization, consistent with the
    /// rest of the codebase.
    /// </summary>
    /// <typeparam name="T">
    /// Preferences type – must be a reference type with a public
    /// parameterless constructor.
    /// </typeparam>
    public class PreferencesRepository<T> : RepositoryBase<T> where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PreferencesRepository{T}"/>.
        /// </summary>
        /// <param name="logger">Optional logger.</param>
        public PreferencesRepository(ILogger logger = null) : base(logger) { }

        /// <summary>
        /// Loads preferences from an XML file at <paramref name="filePath"/>.
        /// Returns a default instance when the file is absent or unreadable.
        /// </summary>
        public override T Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogWarning("PreferencesRepository.Load called with null/empty path");
                return null;
            }

            T result = XmlUtils.XmlFileLoad<T>(filePath);

            if (result == null)
            {
                Logger.LogWarning("Could not load preferences from {FilePath} – returning defaults", filePath);
                result = new T();
            }

            return result;
        }

        /// <summary>
        /// Saves <paramref name="entity"/> to an XML file at <paramref name="filePath"/>.
        /// </summary>
        public override bool Save(T entity, string filePath)
        {
            if (entity == null)
            {
                Logger.LogError("PreferencesRepository.Save: entity is null");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogError("PreferencesRepository.Save: filePath is null/empty");
                return false;
            }

            bool success = XmlUtils.XmlFileSave(entity, filePath);

            if (!success)
            {
                Logger.LogError("PreferencesRepository failed to save preferences to {FilePath}", filePath);
            }

            return success;
        }

        /// <inheritdoc/>
        public override T GetDefault() => new T();

        /// <summary>
        /// Asynchronously loads preferences from an XML file at <paramref name="filePath"/>
        /// using <see cref="XmlUtils.XmlFileLoadAsync{T}"/> to avoid blocking the calling thread.
        /// Returns a default instance when the file is absent or unreadable.
        /// </summary>
        public override async Task<T> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogWarning("PreferencesRepository.LoadAsync called with null/empty path");
                return null;
            }

            T result = await XmlUtils.XmlFileLoadAsync<T>(filePath, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                Logger.LogWarning("Could not load preferences from {FilePath} – returning defaults", filePath);
                result = new T();
            }

            return result;
        }

        /// <summary>
        /// Asynchronously saves <paramref name="entity"/> to an XML file at <paramref name="filePath"/>
        /// using <see cref="XmlUtils.XmlFileSaveAsync{T}"/> to avoid blocking the calling thread.
        /// </summary>
        public override async Task<bool> SaveAsync(T entity, string filePath, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                Logger.LogError("PreferencesRepository.SaveAsync: entity is null");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogError("PreferencesRepository.SaveAsync: filePath is null/empty");
                return false;
            }

            bool success = await XmlUtils.XmlFileSaveAsync(entity, filePath, cancellationToken).ConfigureAwait(false);

            if (!success)
            {
                Logger.LogError("PreferencesRepository failed to save preferences to {FilePath}", filePath);
            }

            return success;
        }
    }
}
