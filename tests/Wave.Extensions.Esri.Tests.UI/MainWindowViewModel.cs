using System;
using System.Collections.Generic;
using System.Windows;

using ESRI.ArcGIS.Geodatabase;

using Wave.Extensions.Esri.Tests.UI.Control.AutoCompleteTextBox;
using Wave.Extensions.Esri.Tests.UI.Control.BusyIndicator;
using Wave.Extensions.Esri.Tests.UI.Control.TokenizedTextBox;
using Wave.Extensions.Esri.Tests.UI.Control.Watermark;

namespace Wave.Extensions.Esri.Tests.UI
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.TokenizedTextBoxCommand = new DelegateCommand((o) => this.ShowWindow(new TokenizedTextBoxView()));
            this.WatermarksCommand = new DelegateCommand((o) => this.ShowWindow(new WatermarkView()));
            this.AutoCompleteCommand = new DelegateCommand(o => this.ShowWindow(new AutoCompleteTextBoxView()));
            this.BusyIndicatorCommand = new DelegateCommand(o => this.ShowWindow(new BusyIndicatorView()));
            this.ChangeVersionCommand = new DelegateCommand(o => this.ShowWindow(new ChangeVersionDialog(this.GetVersions())));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the automatic complete command.
        /// </summary>
        /// <value>
        ///     The automatic complete command.
        /// </value>
        public DelegateCommand AutoCompleteCommand { get; private set; }

        /// <summary>
        ///     Gets the tab control command.
        /// </summary>
        /// <value>
        ///     The tab control command.
        /// </value>
        public DelegateCommand BusyIndicatorCommand { get; private set; }

        /// <summary>
        ///     Gets the change version command.
        /// </summary>
        /// <value>
        ///     The change version command.
        /// </value>
        public DelegateCommand ChangeVersionCommand { get; private set; }

        /// <summary>
        ///     Gets the tokenized text box command.
        /// </summary>
        /// <value>
        ///     The tokenized text box command.
        /// </value>
        public DelegateCommand TokenizedTextBoxCommand { get; private set; }

        /// <summary>
        ///     Gets the watermarks command.
        /// </summary>
        /// <value>
        ///     The watermarks command.
        /// </value>
        public DelegateCommand WatermarksCommand { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the versions.
        /// </summary>
        /// <returns></returns>
        private List<VersionInfo> GetVersions()
        {
            List<VersionInfo> list = new List<VersionInfo>();
            list.Add(new VersionInfo("JSMITH.MM_1234", "", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-9)));
            list.Add(new VersionInfo("AJOHNSON.MM_22192", "", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-9)));
            list.Add(new VersionInfo("JSMITH.EDIT_9182", "", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-5)));
            list.Add(new VersionInfo("AJOHNSON.EDIT_6564", "", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-5)));
            return list;
        }

        /// <summary>
        ///     Shows the tokenized text box window.
        /// </summary>
        private void ShowWindow(Window window)
        {
            window.ShowDialog();
        }

        #endregion
    }

    class VersionInfo : IVersionInfo
    {
        #region Constructors

        public VersionInfo(string versionName, string description, DateTime? created, DateTime? modified)
        {
            this.VersionName = versionName;
            this.Description = description;
            this.Created = created;
            this.Modified = modified;
            this.Access = esriVersionAccess.esriVersionAccessPublic;
        }

        #endregion

        #region Public Properties

        public esriVersionAccess Access { get; private set; }
        public IEnumVersionInfo Ancestors { get; private set; }
        public IEnumVersionInfo Children { get; private set; }
        public object Created { get; private set; }
        public string Description { get; private set; }
        public object Modified { get; private set; }
        public IVersionInfo Parent { get; private set; }

        public string VersionName { get; private set; }

        #endregion

        #region IVersionInfo Members

        public bool IsOwner()
        {
            return true;
        }

        #endregion
    }
}