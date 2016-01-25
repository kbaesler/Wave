using System.Collections.Generic;
using System.Windows;

using Wave.Searchability.Data;

namespace Wave.Searchability.Events
{
    internal class SearchableInventoryEvent : CompositePresentationEvent<IEnumerable<SearchableInventory>>
    {
    }
}