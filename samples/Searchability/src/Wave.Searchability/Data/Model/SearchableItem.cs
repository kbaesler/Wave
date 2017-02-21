using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract for representing a searchable item and it's corresponding fields and relationships.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "item", Namespace = "")]
    [KnownType(typeof(SearchableTable))]
    [KnownType(typeof(SearchableLayer))]
    public abstract class SearchableItem : Searchable
    {
        #region Fields

        private List<string> _Path;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        protected SearchableItem(string name, string aliasName)
            : base(name, aliasName)
        {
            this.Relationships = new ObservableCollection<SearchableRelationship>();
            this.Relationships.CollectionChanged += (sender, args) => _Path = null;
            this.Fields = new ObservableCollection<SearchableField>();
            this.ItemType = SearchableItemType.Unknown;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected SearchableItem(string name)
            : this(name, name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        protected SearchableItem(string name, params SearchableField[] fields)
            : this(name)
        {
            this.Fields.AddRange(fields);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="relationships">The relationships.</param>
        protected SearchableItem(string name, IEnumerable<SearchableField> fields, params SearchableRelationship[] relationships)
            : this(name)
        {
            this.Fields.AddRange(fields);
            this.Relationships.AddRange(relationships);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="relationships">The relationships.</param>
        protected SearchableItem(string name, params SearchableRelationship[] relationships)
            : this(name)
        {
            this.Relationships.AddRange(relationships);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the fields.
        /// </summary>
        /// <value>
        ///     The fields.
        /// </value>
        [DataMember(Name = "fields")]
        [Browsable(false)]
        public ObservableCollection<SearchableField> Fields { get; set; }

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        [IgnoreDataMember]
        [Browsable(false)]
        public List<string> Path
        {
            get { return _Path ?? (_Path = this.GetSearchableItemPath(this)); }
        }

        /// <summary>
        ///     Gets or sets the relationships.
        /// </summary>
        /// <value>
        ///     The relationships.
        /// </value>
        [DataMember(Name = "relationships")]
        [Browsable(false)]
        public ObservableCollection<SearchableRelationship> Relationships { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        [DataMember(Name = "itemType")]
        [Browsable(false)]
        public SearchableItemType ItemType { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the hierarchical path of the child relationships.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="string" /> representing the hierarchical path of the item.</returns>
        private List<string> GetSearchableItemPath(SearchableItem source)
        {
            List<string> list = new List<string>();
            list.Add(source.Name);

            foreach (var item in source.Relationships)
            {
                List<string> path = this.GetSearchableItemPath(item);
                list.AddRange(path);
            }

            return list;
        }

        #endregion
    }

    /// <summary>
    ///     An enumeration that indicates the type of the item.
    /// </summary>
    public enum SearchableItemType
    {
        Annotation = 1,
        Dimension = 2,
        Line = 3,
        Point = 4,
        Polygon = 5,
        Table = 6,
        Unknown = -1,
    }
}