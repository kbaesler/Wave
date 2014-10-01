using System.Drawing;
using System.Windows.Forms;

namespace System.Forms
{
    /// <summary>
    ///     Provides extension methods for the <see cref="System.Windows.Forms.ComboBox" /> control.
    /// </summary>
    public static class ComboBoxExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Resizes the control to fit all of the contents.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="appendWidth">The width that is appended.</param>
        public static void ResizeToFit(this ComboBox control, int appendWidth)
        {
            string test = null;
            foreach (object o in control.Items)
                if (test == null || o.ToString().Length > test.Length)
                    test = o.ToString();

            if (test == null) return;

            // Create a Graphics object for the Control.
            using (Graphics g = control.CreateGraphics())
            {
                // Get the Size needed to accommodate the formatted Text.
                Size preferredSize = g.MeasureString(test, control.Font).ToSize();

                // Pad the text and resize the control.
                control.Size = new Size(preferredSize.Width + 12*2 + appendWidth, preferredSize.Height + 10*2);
            }
        }

        /// <summary>
        ///     Resizes the control to fit all of the contents.
        /// </summary>
        /// <param name="control">The control.</param>
        public static void ResizeToFit(this ComboBox control)
        {
            control.ResizeToFit(0);
        }

        #endregion
    }
}