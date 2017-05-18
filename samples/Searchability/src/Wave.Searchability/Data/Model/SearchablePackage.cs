using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract that represents a "set" or "grouping" of searchable items.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "package", Namespace = "")]
    public class SearchablePackage : Searchable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchablePackage"/> class.
        /// </summary>
        public SearchablePackage()
            : base("")
        {
            
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchablePackage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        public SearchablePackage(string name, string aliasName)
            : base(name, aliasName)
        {
            this.Items = new ObservableCollection<SearchableItem>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchablePackage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="items">The items.</param>
        public SearchablePackage(string name, IEnumerable<SearchableItem> items)
            : this(name)
        {
            this.Items.AddRange(items);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchablePackage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchablePackage(string name)
            : base(name, name)
        {
            this.Items = new ObservableCollection<SearchableItem>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchablePackage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="items">The items.</param>
        public SearchablePackage(string name, params SearchableItem[] items)
            : this(name, name, items)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchablePackage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        /// <param name="items">The items.</param>
        public SearchablePackage(string name, string aliasName, params SearchableItem[] items)
            : this(name, aliasName)
        {
            this.Items.AddRange(items);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        [DataMember(Name = "items")]
        [Browsable(false)]
        public ObservableCollection<SearchableItem> Items { get; set; }

        #endregion
    }
}