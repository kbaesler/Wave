using System;

namespace ESRI.ArcGIS.Framework
{

    #region Enumerations

    /// <summary>
    ///     Represents the image used to paint the mouse pointer.
    /// </summary>
    public enum MouseCursorImage
    {
        /// <summary>
        ///     An arrow cursor.
        /// </summary>
        Arrow = 0,

        /// <summary>
        ///     An IBeam cursor.
        /// </summary>
        Beam = 1,

        /// <summary>
        ///     A Wait cursor.
        /// </summary>
        Wait = 2,

        /// <summary>
        ///     A Cross cursor.
        /// </summary>
        Cross = 3,

        /// <summary>
        ///     A UpArrow cursor.
        /// </summary>
        UpArrow = 4,

        /// <summary>
        ///     A SizeAll cursor.
        /// </summary>
        SizeAll = 5,

        /// <summary>
        ///     A SizeNWSE cursor.
        /// </summary>
        SizeNWSE = 6,

        /// <summary>
        ///     A SizeNESW cursor.
        /// </summary>
        SizeNESW = 7,

        /// <summary>
        ///     A SizeWE cursor.
        /// </summary>
        SizeWE = 8,

        /// <summary>
        ///     A SizeNS cursor.
        /// </summary>
        SizeNS = 9,

        /// <summary>
        ///     Nothing.
        /// </summary>
        None = 10
    }

    #endregion

    /// <summary>
    ///     A supporting class used to change the mouse pointer in ArcMap.
    /// </summary>
    public class MouseCursorReverter : IDisposable
    {
        #region Fields

        private readonly IMouseCursor _Cursor;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MouseCursorReverter" /> class.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        public MouseCursorReverter(MouseCursorImage cursor)
            : this((int) cursor)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MouseCursorReverter" /> class.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        public MouseCursorReverter(int cursor)
        {
            _Cursor = new MouseCursorClass();
            _Cursor.SetCursor(cursor);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Cursor != null)
                {
                    _Cursor.SetCursor(MouseCursorImage.Arrow);
                }
            }
        }

        #endregion
    }
}