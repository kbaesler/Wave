using System.Windows.Interop;

namespace System.Windows.Controls
{
    /// <summary>
    ///     Interop Enabled TextBox : This TextBox will properly handle WM_GETDLGCODE Messages allowing Key Input
    /// </summary>
    public class InteropTextBox : TextBox
    {
        #region Constants

        private const uint DLGC_HASSETSEL = 0x0008;
        private const uint DLGC_WANTALLKEYS = 0x0004;
        private const uint DLGC_WANTARROWS = 0x0001;
        private const uint DLGC_WANTCHARS = 0x0080;
        private const uint WM_GETDLGCODE = 0x0087;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="InteropTextBox" /> class.
        /// </summary>
        static InteropTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InteropTextBox), new FrameworkPropertyMetadata(typeof (InteropTextBox)));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InteropTextBox" /> class.
        /// </summary>
        public InteropTextBox()
        {
            Loaded += delegate
            {
                HwndSource s = PresentationSource.FromVisual(this) as HwndSource;
                if (s != null)
                    s.AddHook(ChildHwndSourceHook);
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the  <see cref="HwndSourceHook" /> evnent for the HWND source hook.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns></returns>
        private IntPtr ChildHwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETDLGCODE)
            {
                handled = true;
                return new IntPtr(DLGC_WANTALLKEYS | DLGC_WANTCHARS | DLGC_WANTARROWS | DLGC_HASSETSEL);
            }
            return IntPtr.Zero;
        }

        #endregion
    }
}