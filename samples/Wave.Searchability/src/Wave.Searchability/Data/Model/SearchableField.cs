using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    [DataContract(Name = "field", Namespace = "")]
    public class SearchableField : Searchable
    {
        #region Fields

        private string _Value;
        private bool _Visible;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableField" /> class.
        /// </summary>
        public SearchableField()
            : base(Any)
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
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [DataMember(Name = "value")]
        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;

                this.OnPropertyChanged("Value");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="SearchableField" /> is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "visible")]
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;

                this.OnPropertyChanged("Visible");
            }
        }

        #endregion

        #region Public Methods

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