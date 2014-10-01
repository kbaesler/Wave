using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     An interface used to notify the dictionary is changing
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    public interface INotifyDictionaryChanging<TKey, TValue>
    {
        #region Events

        /// <summary>
        ///     Occurs when the dictionary is changing.
        /// </summary>
        event EventHandler<NotifyDictionaryChangingEventArgs<TKey, TValue>> DictionaryChanging;

        #endregion
    }
}