using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    /// The controller for the <see cref="ChangeVersionDialog"/> control.
    /// </summary>
    /// <seealso cref="System.Windows.BaseViewModel" />
    class ChangeVersionDialogViewModel : BaseViewModel
    {
        #region Fields

        private string _Name;
        private string _Owner;
        private List<string> _Owners;
        private VersionInfo _Version;
        private ICollectionView _Versions;
        private IWorkspace _Workspace;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeVersionDialogViewModel"/> class.
        /// </summary>
        public ChangeVersionDialogViewModel()
            : base("Change Version")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the selected version.
        /// </summary>
        /// <value>
        ///     The selected version.
        /// </value>
        public IVersion SelectedVersion
        {
            get
            {
                if (this.Version == null || this.Workspace == null)
                    return null;

                return ((IVersionedWorkspace) this.Workspace).GetVersion(this.Version.VersionName);
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;

                OnPropertyChanged(() => Name);

                ApplyFilter(value, false);
            }
        }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>
        ///     The owner.
        /// </value>
        public string Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;

                OnPropertyChanged(() => Owner);

                ApplyFilter(value, true);
            }
        }

        /// <summary>
        ///     Gets or sets the owners.
        /// </summary>
        /// <value>
        ///     The owners.
        /// </value>
        public List<string> Owners
        {
            get { return _Owners; }
            set
            {
                _Owners = value;

                OnPropertyChanged(() => Owners);
            }
        }

        /// <summary>
        ///     Gets or sets the version information.
        /// </summary>
        /// <value>
        ///     The version information.
        /// </value>
        public VersionInfo Version
        {
            get { return _Version; }
            set
            {
                _Version = value;

                OnPropertyChanged(() => Version);
            }
        }

        /// <summary>
        ///     Gets or sets the versions.
        /// </summary>
        /// <value>
        ///     The versions.
        /// </value>
        public ICollectionView Versions
        {
            get { return _Versions; }
            set
            {
                _Versions = value;

                OnPropertyChanged(() => Versions);

                var list = new List<string>();
                list.Add("");
                list.AddRange(value.SourceCollection.OfType<VersionInfo>().DistinctBy(o => o.Owner).Select(o => o.Owner));

                this.Owners = list;
            }
        }

        /// <summary>
        ///     Gets or sets the workspace.
        /// </summary>
        /// <value>
        ///     The workspace.
        /// </value>
        public IWorkspace Workspace
        {
            get { return _Workspace; }
            set
            {
                _Workspace = value;

                OnPropertyChanged(() => Workspace);

                var source = ((IVersionedWorkspace) value).Versions.AsEnumerable().Select(o => new VersionInfo(o));
                this.Versions = CollectionViewSource.GetDefaultView(source);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="owner">if set to <c>true</c> the owner property initiated the filter.</param>
        private void ApplyFilter(string value, bool owner)
        {
            if (this.Versions == null)
                return;

            this.Versions.Filter += o =>
            {
                VersionInfo version = o as VersionInfo;
                if (version == null)
                    return true;

                if (owner)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        if (string.IsNullOrEmpty(this.Name))
                            return true;

                        return version.Name.StartsWith(this.Name, StringComparison.OrdinalIgnoreCase);
                    }

                    if (string.IsNullOrEmpty(this.Name))
                        return version.Owner.StartsWith(value, StringComparison.OrdinalIgnoreCase);

                    return version.Owner.StartsWith(value, StringComparison.OrdinalIgnoreCase)
                           && version.Name.StartsWith(this.Name, StringComparison.OrdinalIgnoreCase);
                }
                
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(this.Owner))
                        return true;

                    return version.Owner.StartsWith(this.Owner, StringComparison.OrdinalIgnoreCase);
                }

                if (string.IsNullOrEmpty(this.Owner))
                    return version.Name.StartsWith(value, StringComparison.OrdinalIgnoreCase);

                return version.Owner.StartsWith(this.Owner, StringComparison.OrdinalIgnoreCase)
                       && version.Name.StartsWith(value, StringComparison.OrdinalIgnoreCase);
            };
        }

        #endregion
    }

    /// <summary>
    /// An observable object for the <see cref="IVersionInfo"/>
    /// </summary>
    class VersionInfo : Observable, IVersionInfo
    {
        #region Fields

        private readonly IVersionInfo _DeclaringType;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionInfo" /> class.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        public VersionInfo(IVersionInfo declaringType)
        {
            _DeclaringType = declaringType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The version's access permission.
        /// </summary>
        public esriVersionAccess Access
        {
            get { return _DeclaringType.Access; }
        }

        /// <summary>
        ///     The version's ancestors.
        /// </summary>
        public IEnumVersionInfo Ancestors
        {
            get { return _DeclaringType.Ancestors; }
        }

        /// <summary>
        ///     The version's children.
        /// </summary>
        public IEnumVersionInfo Children
        {
            get { return _DeclaringType.Children; }
        }

        /// <summary>
        ///     The date and time the version was created.
        /// </summary>
        public object Created
        {
            get { return _DeclaringType.Created; }
        }

        /// <summary>
        ///     The version's description.
        /// </summary>
        public string Description
        {
            get { return _DeclaringType.Description; }
        }

        /// <summary>
        /// The date and time the version was last modified.
        /// </summary>
        public object Modified
        {
            get { return _DeclaringType.Modified; }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get
            {
                var index = this.VersionName.IndexOf(".", StringComparison.Ordinal);
                if (index > 0)
                {
                    return this.VersionName.Substring(index + 1, this.VersionName.Length - index - 1);
                }

                return this.VersionName;
            }
        }

        /// <summary>
        ///     Gets the owner.
        /// </summary>
        /// <value>
        ///     The owner.
        /// </value>
        public string Owner
        {
            get
            {
                var index = this.VersionName.IndexOf(".", StringComparison.Ordinal);
                if (index > 0)
                {
                    return this.VersionName.Substring(0, index);
                }

                return this.VersionName;
            }
        }

        /// <summary>
        ///     The version's parent.
        /// </summary>
        public IVersionInfo Parent
        {
            get { return new VersionInfo(_DeclaringType.Parent); }
        }

        /// <summary>
        ///     The version's name.
        /// </summary>
        public string VersionName
        {
            get { return _DeclaringType.VersionName; }
        }

        #endregion

        #region IVersionInfo Members

        /// <summary>
        ///     True if the current connected user is the owner of this version.
        /// </summary>
        /// <returns></returns>
        public bool IsOwner()
        {
            return _DeclaringType.IsOwner();
        }

        #endregion
    }
}