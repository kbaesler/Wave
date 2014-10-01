using System.Diagnostics;
using System.Native;
using System.Threading;
using System.Windows.Forms;

namespace System.Forms
{
    /// <summary>
    ///     Provides the extension methods for the <see cref="System.Windows.Forms.Form" /> class.
    /// </summary>
    public static class ApplicationMutex
    {
        #region Public Methods

        /// <summary>
        ///     Runs the application with the specified <paramref name="form" /> a single instance application.
        /// </summary>
        /// <param name="form">The form.</param>
        public static void RunAsSingleton(Form form)
        {
            bool createdNew;

            // Use a Mutex to make this a single instance application.
            using (new Mutex(true, Application.ProductName, out createdNew))
            {
                if (createdNew)
                {
                    Application.Run(form);
                }
                else
                {
                    // Display the application that is already running by using Native API methods.
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            UnsafeWindowMethods.ShowWindow(process.MainWindowHandle, UnsafeWindowMethods.WindowShowStyle.Restore);
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}