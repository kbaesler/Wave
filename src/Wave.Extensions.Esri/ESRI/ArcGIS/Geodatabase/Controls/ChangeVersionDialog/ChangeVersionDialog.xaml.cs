using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Interaction logic for ChangeVersionDialog.xaml
    /// </summary>
    public partial class ChangeVersionDialog : Window, IChangeVersionDialog2
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeVersionDialog" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        public ChangeVersionDialog(string title)
            : this()
        {
            ((ChangeVersionDialogViewModel) this.DataContext).DisplayName = title;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeVersionDialog" /> class.
        /// </summary>
        public ChangeVersionDialog()
        {
            InitializeComponent();

            this.DataContext = this.DataContext as ChangeVersionDialogViewModel ?? new ChangeVersionDialogViewModel();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeVersionDialog" /> class.
        /// </summary>
        /// <param name="versions">The versions.</param>
        public ChangeVersionDialog(IEnumerable<IVersionInfo> versions)
            : this()
        {
            ChangeVersionDialogViewModel dataContext = (ChangeVersionDialogViewModel) this.DataContext;
            dataContext.Close += (sender, args) => this.Close();
            dataContext.Versions = CollectionViewSource.GetDefaultView(versions.Select(o => new VersionInfo(o)));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeVersionDialog" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="versions">The versions.</param>
        public ChangeVersionDialog(string title, IEnumerable<IVersionInfo> versions)
            : this(versions)
        {
            ((ChangeVersionDialogViewModel) this.DataContext).DisplayName = title;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The selected version.
        /// </summary>
        public IVersion SelectedVersion
        {
            get { return ((ChangeVersionDialogViewModel) this.DataContext).SelectedVersion; }
        }

        #endregion

        #region IChangeVersionDialog2 Members

        /// <summary>
        ///     Displays the dialog used to create new versions in a versioned geodatabase.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public bool DoModal(IWorkspace workspace)
        {
            ChangeVersionDialogViewModel dataContext = (ChangeVersionDialogViewModel) this.DataContext;
            dataContext.Close += (sender, args) => this.Close();
            dataContext.Workspace = workspace;

            var hWnd = ArcMap.Application.GetNativeWindow();
            return this.ShowDialog(hWnd).GetValueOrDefault(false);
        }

        /// <summary>
        ///     Displays the dialog used to create new versions in a versioned geodatabase.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="versions">The versions.</param>
        /// <returns></returns>
        public bool DoModal(IWorkspace workspace, IEnumerable<IVersionInfo> versions)
        {
            ChangeVersionDialogViewModel dataContext = (ChangeVersionDialogViewModel) this.DataContext;
            dataContext.Close += (sender, args) => this.Close();
            dataContext.Workspace = workspace;
            dataContext.Versions = CollectionViewSource.GetDefaultView(versions.Select(o => new VersionInfo(o)));

            var hWnd = ArcMap.Application.GetNativeWindow();
            return this.ShowDialog(hWnd).GetValueOrDefault(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the OnClick event of the No control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void No_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        ///     Handles the OnClick event of the Yes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Yes_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        #endregion
    }
}