using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     A Base object to assist developing against COM objects that require a deterministic release pattern.
    /// </summary>
    [Serializable]
    public class ComReleaser : IDisposable
    {
        #region Fields

        private readonly ArrayList _Array;

        #endregion

        #region Constructors

        /// <summary>Default Constructor</summary>
        public ComReleaser()
        {
            _Array = ArrayList.Synchronized(new ArrayList());
        }

        #endregion

        #region Destructors

        /// <summary>Destructor</summary>
        ~ComReleaser()
        {
            this.Dispose(true);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Implementation of IDisposable method Dispose()
        ///     The Dispose method should release all the resources that it owns for unmanaged code resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Decrements the reference count to zero of the supplied runtime callable wrapper.
        /// </summary>
        /// <param name="o">The COM object to release.</param>
        public static void FinalReleaseComObject(object o)
        {
            if (o == null || !Marshal.IsComObject(o))
                return;

            Marshal.FinalReleaseComObject(o);
        }

        /// <summary>
        ///     Manages the lifetime of any COM object.  The method will deterministically release the object during the dispose
        ///     process.
        /// </summary>
        /// <remarks>
        ///     Marshal.ReleaseComObject will be called during the disposal process on this Interface pointer until its RCW
        ///     reference count becomes 0.
        ///     NOTE: Do not add ServerObject interfaces like IMapServer, IGeocodeServer, IMapServerLayout or IMapServerObjects.
        /// </remarks>
        /// <param name="o">The COM object to manage.</param>
        public void ManageLifetime(object o)
        {
            _Array.Add(o);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Dispose method implementation from IDisposable Interface
        /// </summary>
        /// <param name="disposing">
        ///     Boolean value indicating to the method whether
        ///     or not it should also dispose managed objects
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (var o in _Array)
            {
                FinalReleaseComObject(o);
            }

            _Array.Clear();

            GC.Collect();
        }

        #endregion
    }
}