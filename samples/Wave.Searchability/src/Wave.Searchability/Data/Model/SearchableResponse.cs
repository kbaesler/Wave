using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Wave.Searchability.Data
{
    [CollectionDataContract(Name = "response")]
    public class SearchableResponse : Dictionary<string, List<int>>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableResponse" /> class.
        /// </summary>
        public SearchableResponse()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableResponse" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        internal SearchableResponse(Dictionary<string, List<int>> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableResponse" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        internal SearchableResponse(ConcurrentDictionary<string, ConcurrentBag<int>> dictionary)
            : base(dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()))
        {
        }

        #endregion
    }
}