using System;
using System.Collections.Generic;
using System.Linq;

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

        private readonly Dictionary<int, Dictionary<int, List<Guid>>> _Guids;
        private static InoperableAutoUpdaters _Instance;

        #endregion

        #region Constructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="InoperableAutoUpdaters" /> class from being created.
        /// </summary>
        private InoperableAutoUpdaters()
        {
            _Guids = new Dictionary<int, Dictionary<int, List<Guid>>>();
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
            foreach (var entry in source.GetSubtypes())
                this.Add(source.ObjectClassID, entry.Key, type);
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
            var subtypes = source.GetSubtypes();
            var items = subtypes as KeyValuePair<int, string>[] ?? subtypes.ToArray();
            var flags = items.Select(entry => this.Contains(source.ObjectClassID, entry.Key, type)).ToList();

            return flags.Count == items.Count();
        }

        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        public void Remove(IObjectClass source, Type type)
        {
            foreach (var entry in source.GetSubtypes())
                this.Remove(source.ObjectClassID, entry.Key, type);
        }

        /// <summary>
        ///     Determines whether the type is inoperable.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the type is inoperable for the id.
        /// </returns>
        public bool Contains(int id, int subtype, Type type)
        {
            var subtypes = new Dictionary<int, List<Guid>>();

            if (_Guids.ContainsKey(id))
                subtypes = _Guids[id];

            var list = new List<Guid>();

            if (subtypes.ContainsKey(subtype))
                list = subtypes[subtype];

            return list.Contains(type.GUID);
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            _Guids.Clear();
        }

        /// <summary>
        ///     Adds the specified type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="type">The type.</param>
        public void Add(int id, int subtype, Type type)
        {
            var subtypes = new Dictionary<int, List<Guid>>();

            if (_Guids.ContainsKey(id))
                subtypes = _Guids[id];
            else
                _Guids.Add(id, subtypes);

            var list = new List<Guid>();

            if (subtypes.ContainsKey(subtype))
                list = subtypes[subtype];
            else
                subtypes.Add(subtype, list);

            list.Add(type.GUID);
        }

        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="type">The type.</param>
        public void Remove(int id, int subtype, Type type)
        {
            var subtypes = new Dictionary<int, List<Guid>>();

            if (_Guids.ContainsKey(id))
                subtypes = _Guids[id];

            var list = new List<Guid>();

            if (subtypes.ContainsKey(subtype))
                list = subtypes[subtype];

            list.Remove(type.GUID);
        }

        #endregion
    }
}