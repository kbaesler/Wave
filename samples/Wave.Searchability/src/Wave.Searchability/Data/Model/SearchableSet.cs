using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract that represents a "set" or "grouping" of searchable items.
    /// </summary>
    [DataContract(Name = "set", Namespace = "")]
    public class SearchableSet : Searchable
    {
        #region Fields

        private ObservableCollection<SearchableTable> _Tables;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableSet" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableSet(string name)
            : base(name)
        {
            _Tables = new ObservableCollection<SearchableTable>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableSet" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tables">The tables.</param>
        public SearchableSet(string name, params SearchableTable[] tables)
            : this(name)
        {
            _Tables.AddRange(tables);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the tables.
        /// </summary>
        /// <value>
        ///     The tables.
        /// </value>
        [DataMember(Name = "tables")]
        public ObservableCollection<SearchableTable> Tables
        {
            get { return _Tables; }
            set
            {
                _Tables = value;

                base.OnPropertyChanged("tables");
            }
        }

        #endregion
    }
}