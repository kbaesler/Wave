using Microsoft.Win32.SafeHandles;

namespace System.Native
{
    /// <summary>
    /// </summary>
    /// <seealso cref="Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid" />
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Constructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="SafeTokenHandle" /> class from being created.
        /// </summary>
        private SafeTokenHandle()
            : base(true)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        ///     true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this
        ///     case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            return UnsafeWindowMethods.CloseHandle(handle);
        }

        #endregion
    }
}