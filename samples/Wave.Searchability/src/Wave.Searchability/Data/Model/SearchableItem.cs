using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract for representing a searchable item and it's corresponding fields and relationships.
    /// </summary>
    [DataContract(Name = "item", Namespace = "")]
    public abstract class SearchableItem : Searchable
    {
        #region Fields

        private ObservableCollection<SearchableField> _Fields;
        private List<string> _Path;
        private ObservableCollection<SearchableRelationship> _Relationships;        
 
        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected SearchableItem(string name)
            : base(name)
        {
            _Relationships = new ObservableCollection<SearchableRelationship>();
            _Relationships.CollectionChanged += (sender, args) => _Path = null;
            _Fields = new ObservableCollection<SearchableField>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        protected SearchableItem(string name, params SearchableField[] fields)
            : this(name)
        {
            _Fields.AddRange(fields);
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
            _Fields.AddRange(fields);
            _Relationships.AddRange(relationships);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableItem" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="relationships">The relationships.</param>
        protected SearchableItem(string name, params SearchableRelationship[] relationships)
            : this(name)
        {
            _Relationships.AddRange(relationships);
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
        public ObservableCollection<SearchableField> Fields
        {
            get { return _Fields; }
            set
            {
                _Fields = value;

                this.OnPropertyChanged("Fields");
            }
        }

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
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
        public ObservableCollection<SearchableRelationship> Relationships
        {
            get { return _Relationships; }
            set
            {
                _Relationships = value;

                this.OnPropertyChanged("Relationships");
            }
        }

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
}