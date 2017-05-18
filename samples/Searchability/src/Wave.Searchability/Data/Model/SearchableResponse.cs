using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ESRI.ArcGIS.Carto;

namespace Wave.Searchability.Data
{
    /// <summary>
    /// The response returned from the search services.
    /// </summary>
    [CollectionDataContract(Name = "response")]
    public class SearchableResponse : Dictionary<string, List<IFeatureFindData2>>
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
        internal SearchableResponse(Dictionary<string, List<IFeatureFindData2>> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableResponse" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        internal SearchableResponse(ConcurrentDictionary<string, ConcurrentBag<IFeatureFindData2>> dictionary)
            : base(dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()))
        {
        }

        #endregion
    }
}