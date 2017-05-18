using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract for representing a searchable feature class layer.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "layer", Namespace = "")]
    public class SearchableLayer : SearchableTable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableLayer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliasName">Name of the alias.</param>
        public SearchableLayer(string name, string aliasName)
            : base(name, aliasName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableLayer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableLayer(string name)
            : this(name, name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableLayer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        public SearchableLayer(string name, params SearchableField[] fields)
            : base(name, fields)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableLayer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="relationships">The relationships.</param>
        public SearchableLayer(string name, IEnumerable<SearchableField> fields, params SearchableRelationship[] relationships)
            : base(name, fields, relationships)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the layer definition is used.
        /// </summary>
        /// <value>
        ///     <c>true</c> if layer definition is used; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "layerDefinition")]
        [Category("Behavior")]
        [Description("Whether or not the layer definition on the layer should be used when querying data.")]
        public bool LayerDefinition { get; set; }

        #endregion
    }
}