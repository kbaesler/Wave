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
    [DataContract(Name = "inventory", Namespace = "")]
    public class SearchableInventory : Searchable
    {
        #region Fields

        private string _Header;
        private ObservableCollection<SearchableItem> _Items;
        private SearchableInventoryType _Type;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableInventory" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        public SearchableInventory(string name, string aliasName)
            : base(name, aliasName)
        {
            _Items = new ObservableCollection<SearchableItem>();
            _Type = SearchableInventoryType.Unknown;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableInventory" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableInventory(string name)
            : base(name, name)
        {
            _Type = SearchableInventoryType.Unknown;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableInventory" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="items">The items.</param>
        public SearchableInventory(string name, params SearchableItem[] items)
            : this(name, name, items)
        {
            _Type = SearchableInventoryType.Unknown;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableInventory" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        /// <param name="items">The items.</param>
        public SearchableInventory(string name, string aliasName, params SearchableItem[] items)
            : this(name, aliasName)
        {
            _Items.AddRange(items);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        /// <value>
        ///     The header.
        /// </value>
        [DataMember(Name = "header")]
        public string Header
        {
            get { return _Header; }
            set
            {
                _Header = value;

                base.OnPropertyChanged("Header");
            }
        }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
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

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        [DataMember(Name = "type")]
        public SearchableInventoryType Type
        {
            get { return _Type; }
            set
            {
                _Type = value;

                base.OnPropertyChanged("Type");
            }
        }

        #endregion
    }

    /// <summary>
    ///     An enumeration that indicates the type of the inventory.
    /// </summary>
    public enum SearchableInventoryType
    {
        Point = 0,
        Line = 1,
        Polygon = 2,
        Table = 3,
        Annotation = 4,
        Dimension = 5,
        Unknown = -1,
    }
}