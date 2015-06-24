using System.Drawing;
using System.Linq;
using System.Native;
using System.Windows.Forms;

using Microsoft.Win32;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     A window behavior that allows for modifying the <see cref="Window" /> behavior.
    /// </summary>
    /// <remarks>
    ///     This file contains the static class which enables the use of attached properties, and delegates its work to it’s
    ///     nested static class WindowHelper.
    ///     On a low-level the bulk of the work is done by making calls to the Win32 API.
    ///     I have chosen to use 32/64-bit compatible functions to allow for the most compatible solution.
    ///     In addition I have surround the low-level code that modifies the WPF window with lock statements just as a
    ///     precaution, although I don’t expect any problems either way.
    /// </remarks>
    public static class WindowBehavior
    {
        #region Fields

        /// <summary>
        ///     The dependency property that controls if the window can be closed.
        /// </summary>
        public static readonly DependencyProperty CanClose =
            DependencyProperty.RegisterAttached("CanClose", typeof (Visibility), typeof (Window),
                new PropertyMetadata(Visibility.Visible, OnCanCloseChanged));

        /// <summary>
        ///     The dependency property that controls if the window can be maximized.
        /// </summary>
        public static readonly DependencyProperty CanMaximize =
            DependencyProperty.RegisterAttached("CanMaximize", typeof (Visibility), typeof (Window),
                new PropertyMetadata(Visibility.Visible, OnCanMaximizeChanged));

        /// <summary>
        ///     The dependency property that controls if the window can be minimized.
        /// </summary>
        public static readonly DependencyProperty CanMinimize =
            DependencyProperty.RegisterAttached("CanMinimize", typeof (Visibility), typeof (Window),
                new PropertyMetadata(Visibility.Visible, OnCanMinimizeChanged));

        /// <summary>
        ///     The dependency property that controls the registry location for the placement
        /// </summary>
        public static readonly DependencyProperty PlacementRegistry =
            DependencyProperty.RegisterAttached("PlacementRegistry", typeof (string), typeof (Window),
                new PropertyMetadata(null, OnPlacementRegistryChanged));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the CanClose dependency property.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <returns></returns>
        public static Visibility GetCanClose(DependencyObject d)
        {
            return (Visibility) d.GetValue(CanClose);
        }

        /// <summary>
        ///     Gets the CanMaximize dependency property.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <returns></returns>
        public static Visibility GetCanMaximize(DependencyObject d)
        {
            return (Visibility) d.GetValue(CanMaximize);
        }

        /// <summary>
        ///     Gets the CanMinimize dependency property.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <returns></returns>
        public static Visibility GetCanMinimize(DependencyObject d)
        {
            return (Visibility) d.GetValue(CanMinimize);
        }

        /// <summary>
        ///     Gets the placement registry key.
        /// </summary>
        /// <param name="d">The command.</param>
        /// <returns></returns>
        public static string GetPlacementRegistry(DependencyObject d)
        {
            return (string) d.GetValue(PlacementRegistry);
        }

        /// <summary>
        ///     Sets the CanClose dependency property.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCanClose(DependencyObject d, object value)
        {
            d.SetValue(CanClose, value);
        }

        /// <summary>
        ///     Sets the CanMaximize dependency property.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCanMaximize(DependencyObject d, object value)
        {
            d.SetValue(CanMaximize, value);
        }

        /// <summary>
        ///     Sets the CanMinimize dependency property.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCanMinimize(DependencyObject d, object value)
        {
            d.SetValue(CanMinimize, value);
        }

        /// <summary>
        ///     Sets the placement registry key.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetPlacementRegistry(DependencyObject d, object value)
        {
            d.SetValue(PlacementRegistry, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines whether the <paramref name="rectangle" /> location is visible on any of the available
        ///     screens.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the rectangle is visible on any of the available screens; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsVisibleWithinAnyScreen(Rectangle rectangle)
        {
            Screen[] screens = Screen.AllScreens;
            return screens.Any(screen => screen.WorkingArea.Contains(rectangle));
        }

        /// <summary>
        ///     Loads the window placement from the registry.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="name">The name.</param>
        private static void LoadFromRegistry(Window window, string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(name))
            {
                if (key != null)
                {
                    string prefix = window.GetType().Namespace + "." + window.GetType().Name;
                    double left = Convert.ToDouble(key.GetValue(prefix + "_Left", window.Left));
                    double top = Convert.ToDouble(key.GetValue(prefix + "_Top", window.Top));
                    double width = Convert.ToDouble(key.GetValue(prefix + "_Width", window.Width));
                    double height = Convert.ToDouble(key.GetValue(prefix + "_Height", window.Height));

                    WindowState windowState = (WindowState) Convert.ToInt32(key.GetValue(prefix + "_WindowState", (int) window.WindowState));
                    SizeToContent sizeToContent = (SizeToContent) Convert.ToInt32(key.GetValue(prefix + "_SizeToContent", (int) window.SizeToContent));

                    Rectangle rectangle = new Rectangle((int) left, (int) top, (int) width, (int) height);
                    if (IsVisibleWithinAnyScreen(rectangle))
                    {
                        window.Left = left;
                        window.Top = top;
                        window.Height = height;
                        window.Width = width;
                        window.WindowState = windowState;
                        window.SizeToContent = sizeToContent;
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the <see cref="PropertyChangedCallback" /> for the CanClose dependency property.
        /// </summary>
        /// <param name="d">The dependency property.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                RoutedEventHandler loadedHandler = null;
                loadedHandler = delegate
                {
                    lock (window)
                    {
                        UnsafeWindowMethods.SetWindowMenuVisibility(window, (Visibility) e.NewValue);
                    }

                    window.Loaded -= loadedHandler;
                };

                if (!window.IsLoaded)
                {
                    window.Loaded += loadedHandler;
                }
                else
                {
                    loadedHandler(null, null);
                }
            }
        }

        /// <summary>
        ///     Handles the <see cref="PropertyChangedCallback" /> for the CanMaximize dependency property.
        /// </summary>
        /// <param name="d">The dependency property.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnCanMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                RoutedEventHandler loadedHandler = null;
                loadedHandler = delegate
                {
                    lock (window)
                    {
                        UnsafeWindowMethods.SetMaximizeBoxVisibility(window, (Visibility) e.NewValue);
                    }

                    window.Loaded -= loadedHandler;
                };

                if (!window.IsLoaded)
                {
                    window.Loaded += loadedHandler;
                }
                else
                {
                    loadedHandler(null, null);
                }
            }
        }

        /// <summary>
        ///     Handles the <see cref="PropertyChangedCallback" /> for the CanMinimize dependency property.
        /// </summary>
        /// <param name="d">The dependency property.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnCanMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                RoutedEventHandler loadedHandler = null;
                loadedHandler = delegate
                {
                    lock (window)
                    {
                        UnsafeWindowMethods.SetMinimizeBoxVisibility(window, (Visibility) e.NewValue);
                    }

                    window.Loaded -= loadedHandler;
                };

                if (!window.IsLoaded)
                {
                    window.Loaded += loadedHandler;
                }
                else
                {
                    loadedHandler(null, null);
                }
            }
        }

        /// <summary>
        ///     Handles the <see cref="PropertyChangedCallback" /> for the PlacementRegistry dependency property.
        /// </summary>
        /// <param name="d">The dependency property.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnPlacementRegistryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                RoutedEventHandler loadedHandler = null;
                loadedHandler = delegate
                {
                    lock (window)
                    {
                        LoadFromRegistry(window, (string) e.NewValue);
                    }

                    window.Loaded -= loadedHandler;
                };

                EventHandler closedHandler = null;
                closedHandler = delegate
                {
                    lock (window)
                    {
                        SaveToRegistry(window, (string) e.NewValue);
                    }

                    window.Closed -= closedHandler;
                };

                if (!window.IsLoaded)
                {
                    window.Loaded += loadedHandler;
                    window.Closed += closedHandler;
                }
                else
                {
                    loadedHandler(null, null);
                }
            }
        }

        /// <summary>
        ///     Saves the window placement to the registry.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="name">The name.</param>
        private static void SaveToRegistry(Window window, string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(name))
            {
                if (key != null)
                {
                    string prefix = window.GetType().Namespace + "." + window.GetType().Name;
                    key.SetValue(prefix + "_Left", window.Left);
                    key.SetValue(prefix + "_Top", window.Top);
                    key.SetValue(prefix + "_Width", window.Width);
                    key.SetValue(prefix + "_Height", window.Height);
                    key.SetValue(prefix + "_WindowState", (int) window.WindowState);
                    key.SetValue(prefix + "_SizeToContent", (int) window.SizeToContent);
                }
            }
        }

        #endregion
    }
}