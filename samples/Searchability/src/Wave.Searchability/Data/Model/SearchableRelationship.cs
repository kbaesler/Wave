using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract that represents a searchable relationship.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract(Name = "relationship", Namespace = "")]
    public class SearchableRelationship : SearchableItem
    {
        #region Fields

        /// <summary>
        ///     A static instance that represents any relationship.
        /// </summary>
        public new static SearchableRelationship Any = new SearchableRelationship(Searchable.Any);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        public SearchableRelationship()
            : base(Searchable.Any)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SearchableRelationship(string name)
            : base(name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fields">The fields.</param>
        public SearchableRelationship(string name, params SearchableField[] fields)
            : base(name, fields)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="relationships">The relationships.</param>
        public SearchableRelationship(string name, params SearchableRelationship[] relationships)
            : base(name, relationships)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public SearchableRelationship(params SearchableField[] fields)
            : base(Searchable.Any, fields)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        public SearchableRelationship(params SearchableRelationship[] relationships)
            : base(Searchable.Any, relationships)
        {
        }

        #endregion
    }
}