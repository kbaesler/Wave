using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace System.Diagnostics
{
    /// <summary>
    ///     Extension methods for exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the error message and appends the translated exception return codes.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     Returns the message for the error.
        /// </returns>
        public static string GetErrorMessage(this Exception exception)
        {
            COMException com = exception as COMException;
            if (com != null)
            {
                return com.Message + "\nError: " + com.ErrorCode + " (" + com.GetErrorName() + ")";
            }

            return exception.Message;
        }

        /// <summary>
        ///     Gets the name of the error by translating the error code into it's corresponding enumeration.
        /// </summary>
        /// <param name="externalException">The external exception.</param>
        /// <returns>
        ///     Returns the enumeration of the error code otherwise <c>UNSPECIFIED FAILURE</c> is returned.
        /// </returns>
        public static string GetErrorName(this ExternalException externalException)
        {
            if (Enum.IsDefined(typeof (fdoError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (fdoError), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (esriNetworkErrors), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (esriNetworkErrors), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (esriGeometryError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (esriGeometryError), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (dimError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (dimError), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (annoError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (annoError), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (esriCoreErrorReturnCodes), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (esriCoreErrorReturnCodes), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (esriDataConverterError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (esriDataConverterError), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (esriSpatialReferenceError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (esriSpatialReferenceError), externalException.ErrorCode);
            }
            if (Enum.IsDefined(typeof (esriRepresentationDrawingError), externalException.ErrorCode))
            {
                return Enum.GetName(typeof (esriRepresentationDrawingError), externalException.ErrorCode);
            }

            Win32Exception e = new Win32Exception(externalException.ErrorCode);
            return e.Message.ToUpper(CultureInfo.CurrentCulture);
        }

        #endregion
    }
}