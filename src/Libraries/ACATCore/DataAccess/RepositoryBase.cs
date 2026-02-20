////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RepositoryBase.cs
//
// Abstract base class providing common repository functionality.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Abstract base class for in-memory repositories backed by persistent storage.
    /// Subclasses override <see cref="LoadFromStorage"/> and <see cref="SaveToStorage"/>
    /// to provide the concrete persistence strategy.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by this repository.</typeparam>
    /// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        /// <summary>Logger for diagnostic output.</summary>
        protected readonly ILogger Logger;

        private readonly Dictionary<TKey, TEntity> _cache = new Dictionary<TKey, TEntity>();
        private bool _loaded;

        /// <summary>
        /// Initializes a new instance of <see cref="RepositoryBase{TEntity, TKey}"/>.
        /// </summary>
        /// <param name="logger">Logger for diagnostic output.</param>
        protected RepositoryBase(ILogger logger = null)
        {
            Logger = logger;
        }

        /// <inheritdoc />
        public TEntity GetById(TKey id)
        {
            EnsureLoaded();
            _cache.TryGetValue(id, out var entity);
            return entity;
        }

        /// <inheritdoc />
        public IReadOnlyList<TEntity> GetAll()
        {
            EnsureLoaded();
            return new List<TEntity>(_cache.Values).AsReadOnly();
        }

        /// <inheritdoc />
        public void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            EnsureLoaded();
            var key = GetKey(entity);
            _cache[key] = entity;
        }

        /// <inheritdoc />
        public void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            EnsureLoaded();
            var key = GetKey(entity);
            if (!_cache.ContainsKey(key))
                throw new KeyNotFoundException($"Entity with key '{key}' not found.");
            _cache[key] = entity;
        }

        /// <inheritdoc />
        public void Remove(TKey id)
        {
            EnsureLoaded();
            _cache.Remove(id);
        }

        /// <inheritdoc />
        public bool Save()
        {
            try
            {
                var entities = new List<TEntity>(_cache.Values);
                SaveToStorage(entities);
                Logger?.LogDebug("{Repository} saved {Count} entities", GetType().Name, entities.Count);
                return true;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "{Repository} failed to save entities", GetType().Name);
                return false;
            }
        }

        // -----------------------------------------------------------------------
        // Protected abstract members for subclasses to implement
        // -----------------------------------------------------------------------

        /// <summary>
        /// Extracts the unique key from an entity.
        /// </summary>
        protected abstract TKey GetKey(TEntity entity);

        /// <summary>
        /// Loads all entities from the underlying persistent storage.
        /// </summary>
        protected abstract IEnumerable<TEntity> LoadFromStorage();

        /// <summary>
        /// Persists the given entities to the underlying storage.
        /// </summary>
        protected abstract void SaveToStorage(IEnumerable<TEntity> entities);

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        private void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            try
            {
                var entities = LoadFromStorage();
                if (entities != null)
                {
                    foreach (var entity in entities)
                        _cache[GetKey(entity)] = entity;
                }
                Logger?.LogDebug("{Repository} loaded {Count} entities", GetType().Name, _cache.Count);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "{Repository} failed to load entities from storage", GetType().Name);
            }
        }
    }
}
