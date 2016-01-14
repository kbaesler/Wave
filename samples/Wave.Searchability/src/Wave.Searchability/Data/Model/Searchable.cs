using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;

namespace Wave.Searchability.Data
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    [DataContract(Name = "search", Namespace = "")]
    public abstract class Searchable : Observable
    {
        #region Constants

        /// <summary>
        ///     A constant that represents 'Any' or 'All'.
        /// </summary>
        public const string Any = "*";

        #endregion

        #region Fields

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

        #endregion

        #region Public Properties

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
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}