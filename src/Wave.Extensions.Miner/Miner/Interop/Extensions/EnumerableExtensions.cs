using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for ArcGIS COM collections
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumField" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumField" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.</returns>
        public static IEnumerable<IField> AsEnumerable(this IMMEnumField source)
        {
            if (source != null)
            {
                source.Reset();
                IField field = source.Next();
                while (field != null)
                {
                    yield return field;
                    field = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumTable" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumTable" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.</returns>
        public static IEnumerable<ITable> AsEnumerable(this IMMEnumTable source)
        {
            if (source != null)
            {
                source.Reset();
                ITable table = source.Next();
                while (table != null)
                {
                    yield return table;
                    table = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumObjectClass" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumObjectClass" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.</returns>
        public static IEnumerable<IObjectClass> AsEnumerable(this IMMEnumObjectClass source)
        {
            if (source != null)
            {
                source.Reset();
                IObjectClass oclass = source.Next();
                while (oclass != null)
                {
                    yield return oclass;
                    oclass = source.Next();
                }
            }
        }


        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumFeederSource" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumFeederSource" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.</returns>
        public static IEnumerable<IMMFeederSource> AsEnumerable(this IMMEnumFeederSource source)
        {
            if (source != null)
            {
                source.Reset();
                IMMFeederSource feeder = source.Next();
                while (feeder != null)
                {
                    yield return feeder;
                    feeder = source.Next();
                }
            }
        }

        #endregion
    }
}