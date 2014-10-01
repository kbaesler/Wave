using System;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.esriSystem.IUID" /> interface.
    /// </summary>
    public static class UIDExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an instance of the class using the specified <see cref="IUID" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The UID.</param>
        /// <returns>
        ///     The class for the GUID; otherwise null.
        /// </returns>
        public static TValue Create<TValue>(this IUID source)
        {
            if (source == null) return default(TValue);

            // When the type could be located and matches the given type.
            Type t = Type.GetTypeFromCLSID(new Guid(source.Value.ToString()));
            if (t == null) return default(TValue);

            object o = Activator.CreateInstance(t);
            if (o is TValue)
                return (TValue) o;

            return default(TValue);
        }

        #endregion
    }
}