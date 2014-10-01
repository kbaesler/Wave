using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     An interface used to notify the dictionary has changed.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    public interface INotifyDictionaryChanged<TKey, TValue>
    {
        #region Events

        /// <summary>
        ///     Occurs when the dictionary has changed.
        /// </summary>
        event EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>> DictionaryChanged;

        #endregion
    }
}