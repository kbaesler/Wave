using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Framework
{
    /// <summary>
    ///     Provides access to the inoperable auto updaters
    /// </summary>
    /// <seealso cref="Miner.Framework.IInoperableAutoUpdaters" />
    public sealed class InoperableAutoUpdaters : IInoperableAutoUpdaters
    {
        #region Fields

        private readonly Dictionary<int, List<Guid>> _Guids;
        private static InoperableAutoUpdaters _Instance;

        #endregion

        #region Constructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="InoperableAutoUpdaters" /> class from being created.
        /// </summary>
        private InoperableAutoUpdaters()
        {
            _Guids = new Dictionary<int, List<Guid>>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IInoperableAutoUpdaters Instance
        {
            get { return _Instance ?? (_Instance = new InoperableAutoUpdaters()); }
        }

        #endregion

        #region IInoperableAutoUpdaters Members

        /// <summary>
        ///     Adds the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        public void Add(IObjectClass source, Type type)
        {
            this.Add(source.ObjectClassID, type);
        }

        /// <summary>
        ///     Determines whether the type is inoperable.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the type is inoperable for the source.
        /// </returns>
        public bool Contains(IObjectClass source, Type type)
        {
            return this.Contains(source.ObjectClassID, type);
        }

        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        public void Remove(IObjectClass source, Type type)
        {
            this.Remove(source.ObjectClassID, type);
        }

        /// <summary>
        ///     Adds the specified type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        public void Add(int id, Type type)
        {
            var list = new List<Guid>();

            if (_Guids.ContainsKey(id))
                list = _Guids[id];
            else
                _Guids.Add(id, list);

            list.Add(type.GUID);
        }

        /// <summary>
        ///     Determines whether the type is inoperable.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the type is inoperable for the id.
        /// </returns>
        public bool Contains(int id, Type type)
        {
            var list = new List<Guid>();

            if (_Guids.ContainsKey(id))
                list = _Guids[id];

            return list.Contains(type.GUID);
        }

        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        public void Remove(int id, Type type)
        {
            var list = new List<Guid>();

            if (_Guids.ContainsKey(id))
                list = _Guids[id];

            list.Remove(type.GUID);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _Guids.Clear();
        }

        #endregion
    }
}