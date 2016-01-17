using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;

namespace Wave.Searchability.Data
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    [DataContract]
    public abstract class Searchable : Observable
    {
        #region Constants

        /// <summary>
        ///     A constant that represents 'Any' or 'All'.
        /// </summary>
        public const string Any = "*";

        #endregion

        #region Fields

        private string _AliasName;
        private string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Searchable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected Searchable(string name)
        {
            this.Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Searchable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        protected Searchable(string name, string aliasName)
            : this(name)
        {
            this.AliasName = aliasName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the alias.
        /// </summary>
        /// <value>
        ///     The name of the alias.
        /// </value>
        [DataMember(Name = "aliasName")]
        public string AliasName
        {
            get { return _AliasName; }
            set
            {
                _AliasName = value;

                base.OnPropertyChanged("AliasName");
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
        ///     otherwise, false.
        /// </returns>
        /// <param name="obj">
        ///     The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.
        /// </param>
        /// <exception cref="T:System.NullReferenceException">
        ///     The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var item = obj as Searchable;
            if (item == null) return false;

            return item.Name.Equals(this.Name, StringComparison.CurrentCultureIgnoreCase);
        }


        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return this.AliasName ?? this.Name;
        }

        #endregion
    }
}