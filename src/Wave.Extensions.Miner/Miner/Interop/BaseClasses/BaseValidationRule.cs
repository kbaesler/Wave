using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Framework;

using stdole;

namespace Miner.Interop
{
    /// <summary>
    ///     Base class for QA/QC validation rules.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseValidationRule : IMMValidationRule, IMMExtObject, IDisposable
    {
        private static readonly ILog Log = LogProvider.For<BaseValidationRule>();

        #region Fields

        /// <summary>
        ///     Array of class or field model names used to enable the assignment of the validation rule within ArcCatalog.
        /// </summary>
        private readonly string[] _ModelNames;

        /// <summary>
        ///     The name of the validation rule. This name will be displayed in ArcCatalog in the ArcFM Properties
        /// </summary>
        private readonly string _Name;

        /// <summary>
        ///     D8List of the validation errors. Use the AddError method to add errors to this list.
        /// </summary>
        private ID8List _ErrorList;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseValidationRule" /> class.
        /// </summary>
        /// <param name="name">The name of the validation rule. This name will be displayed in ArcCatalog in the ArcFM Properties.</param>
        protected BaseValidationRule(string name)
        {
            _Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseValidationRule" /> class.
        /// </summary>
        /// <param name="name">The name of the validation rule. This name will be displayed in ArcCatalog in the ArcFM Properties.</param>
        /// <param name="modelNames">The model names that must be present on the feature class to be enabled.</param>
        protected BaseValidationRule(string name, params string[] modelNames)
        {
            _Name = name;
            _ModelNames = modelNames;
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

        #region IMMExtObject Members

        /// <summary>
        ///     Gets the bitmap.
        /// </summary>
        /// <value>The bitmap.</value>
        public virtual IPictureDisp Bitmap
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     Gets if this validation rule is enabled.
        /// </summary>
        /// <param name="pvarValues">The parameter values.</param>
        /// <returns><c>true</c> if the validation rule is enabled or visible within ArcCatalog; otherwise <c>false</c>.</returns>
        public virtual bool get_Enabled(ref object pvarValues)
        {
            try
            {
                return this.EnableByModelNames(pvarValues);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error("Error Enabling Validation Rule " + _Name, e);
                else
                    Log.Error(e);
            }

            return false;
        }

        #endregion

        #region IMMValidationRule Members

        /// <summary>
        ///     Determines whether the specified row is valid.
        /// </summary>
        /// <param name="pRow">The row.</param>
        /// <returns>D8List of IMMValidationError items.</returns>
        public virtual ID8List IsValid(IRow pRow)
        {
            // Create a new D8List.
            _ErrorList = new D8ListClass();

            try
            {
                this.InternalIsValid(pRow);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error("Error Executing Validation Rule " + _Name, e);
                else
                    Log.Error(e);
            }

            // Return the error list.
            return _ErrorList;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            MMValidationRules.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMValidationRules.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the error to the internal D8List.
        /// </summary>
        /// <param name="errorMessage">The error message to be added.</param>
        protected void AddError(string errorMessage)
        {
            if (_ErrorList == null)
                _ErrorList = new D8ListClass();

            IMMValidationError error = new MMValidationErrorClass();
            error.Severity = 8;
            error.BitmapID = 0;
            error.ErrorMessage = errorMessage;
            _ErrorList.Add((ID8ListItem) error);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ErrorList != null)
                    while (Marshal.ReleaseComObject(_ErrorList) > 0)
                    {
                        // Loop until reference counter zero.
                    }
            }
        }

        /// <summary>
        ///     Determines if the specified parameter is an object class that has been configured with a class model name
        ///     identified
        ///     in the _ModelNames array.
        /// </summary>
        /// <param name="param">The object class to validate.</param>
        /// <returns>Boolean indicating if the specified object class has any of the appropriate model name(s).</returns>
        protected virtual bool EnableByModelNames(object param)
        {
            if (_ModelNames == null) return true; // No configured model names.
            IObjectClass oclass = param as IObjectClass;
            if (oclass == null) return true;

            return (oclass.IsAssignedClassModelName(_ModelNames));
        }

        /// <summary>
        ///     Internal implementation of the IsValid method. This method is called within internal exception handling to report
        ///     all errors to the event log and prompt the user.
        /// </summary>
        /// <param name="row">The row being validated.</param>
        protected abstract void InternalIsValid(IRow row);

        #endregion
    }
}