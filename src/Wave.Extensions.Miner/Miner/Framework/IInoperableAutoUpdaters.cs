using System;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Framework
{
    /// <summary>
    ///     Provides access to the inoperable auto updaters.
    /// </summary>
    public interface IInoperableAutoUpdaters
    {
        #region Public Methods

        /// <summary>
        /// Adds the specified type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        void Add(int id, Type type);

        /// <summary>
        ///     Adds the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        void Add(IObjectClass source, Type type);

        /// <summary>
        /// Determines whether the type is inoperable.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns>Returns a <see cref="bool"/> representing <c>true</c> when the type is inoperable for the id.</returns>
        bool Contains(int id, Type type);

        /// <summary>
        ///     Determines whether the type is inoperable.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <returns>Returns a <see cref="bool"/> representing <c>true</c> when the type is inoperable for the source.</returns>
        bool Contains(IObjectClass source, Type type);

        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        void Remove(IObjectClass source, Type type);

        /// <summary>
        /// Removes the specified type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        void Remove(int id, Type type);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        #endregion
    }
}