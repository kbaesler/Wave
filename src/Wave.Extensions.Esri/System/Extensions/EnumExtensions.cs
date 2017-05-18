namespace System
{
    /// <summary>
    ///     Provides extension methods for Enum objects.
    /// </summary>
    public static class EnumExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="source">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum source, Enum value)
        {
            if (source == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            // Not as good as the .NET 4 version of this function, but should be good enough
            if (!Enum.IsDefined(source.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), source.GetType()));
            }

            ulong num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(source) & num) == num);
        }

        #endregion
    }
}