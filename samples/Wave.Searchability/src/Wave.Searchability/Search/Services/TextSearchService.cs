using System.Runtime.Serialization;
using System.ServiceModel;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    [ServiceContract]
    public interface ITextSearchService : ISearchableService<TextSearchServiceRequest>
    {
    }

    /// <summary>
    ///     The text-based search service allows for querying a set of table(s), class(es) and relationship(s) using the data
    ///     in the active session.
    /// </summary>
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public sealed class TextSearchService : SearchableService<TextSearchServiceRequest>, ITextSearchService
    {
    }

    /// <summary>
    ///     The requests that are issued to the searchable service.
    /// </summary>
    [DataContract(Name = "request")]
    public class TextSearchServiceRequest : SearchableRequest
    {
    }
}