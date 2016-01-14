using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract that represents a "set" or "grouping" of searchable items.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "set", Namespace = "")]
    public class SearchableSet : Searchable
    {
        #region Fields

        private ObservableCollection<SearchableItem> _Items;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableSet" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableSet(string name)
            : base(name)
        {
            _Items = new ObservableCollection<SearchableItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableSet" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="items">The items.</param>
        public SearchableSet(string name, params SearchableItem[] items)
            : this(name)
        {
            _Items.AddRange(items);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DataMember(Name = "items")]
        public ObservableCollection<SearchableItem> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;

                base.OnPropertyChanged("Items");
            }
        }

        #endregion
    }
}