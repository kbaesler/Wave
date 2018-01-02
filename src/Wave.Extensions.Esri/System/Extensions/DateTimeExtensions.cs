namespace System
{
    /// <summary>
    ///     Provides extension methods for the DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Rounds the DateTime to the highest specific time span.
        /// </summary>
        /// <param name="source">The date.</param>
        /// <param name="span">The span.</param>
        /// <returns>Returns a <see cref="DateTime" /> representing the date.</returns>
        public static DateTime Ceiling(this DateTime source, TimeSpan span)
        {
            long ticks = (source.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        ///     Rounds the DateTime to the lowest specific time span.
        /// </summary>
        /// <param name="source">The date.</param>
        /// <param name="span">The span.</param>
        /// <returns>Returns a <see cref="DateTime" /> representing the date.</returns>
        public static DateTime Floor(this DateTime source, TimeSpan span)
        {
            long ticks = (source.Ticks / span.Ticks);
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        ///     Rounds the DateTime to the nearest specified time span.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="span">The time span.</param>
        /// <returns>Returns a <see cref="DateTime" /> representing the rounded up</returns>
        public static DateTime Round(this DateTime source, TimeSpan span)
        {
            long ticks = (source.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        #endregion
    }
}