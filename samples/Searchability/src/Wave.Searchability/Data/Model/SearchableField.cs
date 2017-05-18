using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Represents a searchable field.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "field", Namespace = "")]
    public class SearchableField : Searchable
    {
        #region Fields

        /// <summary>
        ///     A static instance that represents any field.
        /// </summary>
        public new static SearchableField Any = new SearchableField(Searchable.Any);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableField" /> class.
        /// </summary>
        public SearchableField()
            : base(Searchable.Any)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableField" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableField(string name)
            : base(name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableField" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public SearchableField(string name, string value)
            : base(name)
        {
            this.Value = value;
        }

        #endregion

        #region Public Properties       

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Description("The name of the field.")]
        public override string Name { get; set; }

        /// <summary>
        ///     Gets or sets the name of the alias.
        /// </summary>
        /// <value>
        ///     The name of the alias.
        /// </value>
        /// 
        [Description("The alias name of the field.")]
        public override string AliasName { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [DataMember(Name = "value")]
        [Browsable(false)]
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="SearchableField" /> is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "visible")]
        [Browsable(false)]
        public bool Visible { get; set; }

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
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>
        public override bool Equals(object obj)
        {
            var other = obj as SearchableField;
            if (other == null) return false;

            return other.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}