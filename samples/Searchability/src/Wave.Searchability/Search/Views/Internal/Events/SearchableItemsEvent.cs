using System.Collections.Generic;
using System.Windows;

using Wave.Searchability.Data;

namespace Wave.Searchability.Search.Views
{
    internal sealed class SearchableItemsEvent : CompositePresentationEvent<IEnumerable<Searchable>>
    {
    }    
}