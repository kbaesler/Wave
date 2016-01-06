using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract for representing a searchable table (or feature class).
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "table", Namespace = "")]
    public class SearchableTable : SearchableItem
    {
        #region Fields

        private bool _IsFeatureClass;
        private bool _LayerDefinition;
        private bool _NameAsClassModelName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableTable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableTable(string name)
            : base(name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableTable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        public SearchableTable(string name, params SearchableField[] fields)
            : base(name, fields)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableTable" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="relationships">The relationships.</param>
        public SearchableTable(string name, IEnumerable<SearchableField> fields, params SearchableRelationship[] relationships)
            : base(name, fields, relationships)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is feature class.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is feature class; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "isFeatureClass")]
        public bool IsFeatureClass
        {
            get { return _IsFeatureClass; }
            set
            {
                _IsFeatureClass = value;

                this.OnPropertyChanged("IsFeatureClass");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the layer definition is used.
        /// </summary>
        /// <value>
        ///     <c>true</c> if layer definition is used; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "layerDefinition")]
        public bool LayerDefinition
        {
            get { return _LayerDefinition; }
            set
            {
                _LayerDefinition = value;

                this.OnPropertyChanged("LayerDefinition");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the name property represents a class model name.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the name property represents a class model name; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "nameAsClassModelName")]
        public bool NameAsClassModelName
        {
            get { return _NameAsClassModelName; }
            set
            {
                _NameAsClassModelName = value;

                this.OnPropertyChanged("NameAsClassModelName");
            }
        }

        #endregion
    }
}