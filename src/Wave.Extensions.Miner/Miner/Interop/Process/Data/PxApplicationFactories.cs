using System;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides access to the process framework factories.
    /// </summary>
    public static class PxApplicationFactories
    {
        #region Public Methods

        /// <summary>
        ///     Gets the process framework factory used to create an instance to the process framework.
        /// </summary>
        /// <param name="dbms">The DBMS that corresponds to the type of database.</param>
        /// <returns>
        ///     Returns a <see cref="PxApplicationFactory" /> that is used to create a connection.
        /// </returns>
        /// <exception cref="NotSupportedException">The database is not supported.</exception>
        public static PxApplicationFactory GetFactory(DBMS dbms)
        {
            switch (dbms)
            {
                case DBMS.Access:
                    return new LocalPxApplicationFactory();

                case DBMS.Oracle:
                case DBMS.SqlServer:
                    return new RemotePxApplicationFactory();

                default:
                    throw new NotSupportedException("The database is not supported.");
            }
        }

        #endregion
    }
}