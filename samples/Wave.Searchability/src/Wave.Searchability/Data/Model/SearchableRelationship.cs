using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    /// <summary>
    ///     Provides a data contract that represents a searchable relationship.
    /// </summary>
    [DataContract(Name = "relationship", Namespace = "")]
    public class SearchableRelationship : SearchableItem
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        public SearchableRelationship()
            : base(Any)
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
            : base(Any, fields)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRelationship" /> class.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        public SearchableRelationship(params SearchableRelationship[] relationships)
            : base(Any, relationships)
        {
        }

        #endregion
    }
}