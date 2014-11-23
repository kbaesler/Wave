using System;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxHelper" /> interface.
    /// </summary>
    public static class PxHelperExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the configuration value that is stored in the MM_PX_CONFIG table and converts the value to the specified type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The helper application reference.</param>
        /// <param name="configName">Name of the configuration.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns an object representing the configuration value.
        /// </returns>
        public static TSource GetConfigValue<TSource>(this IMMPxHelper2 source, string configName, TSource fallbackValue)
        {
            if (source == null) return default(TSource);

            object configValue = source.GetConfigValue(configName);

            return TypeCast.Cast(configValue, fallbackValue);
        }

        #endregion
    }
}