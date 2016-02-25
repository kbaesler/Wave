using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Miner.ComCategories;
using Miner.Framework;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Represents an abstract clas for process framework subtask implementations
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Px")]
    [ComVisible(true)]
    public abstract class BasePxSubtask : IMMPxSubtask, IMMPxSubtask2
    {
        #region Fields

        private readonly string _Name;

        private IDictionary _Parameters;
        private IMMPxApplication _PxApp;
        private IMMEnumExtensionNames _SupportedExtensions;
        private IDictionary _SupportedParameters;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxSubtask" /> class.
        /// </summary>
        /// <param name="name">The name </param>
        protected BasePxSubtask(string name)
        {
            _Name = name;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the process application reference.
        /// </summary>
        /// <value>
        ///     The process application reference.
        /// </value>
        protected IMMPxApplication PxApplication
        {
            get { return _PxApp; }
        }

        #endregion

        #region IMMPxSubtask Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        public virtual bool Enabled(IMMPxNode pPxNode)
        {
            try
            {
                return this.InternalEnabled(pPxNode);
            }
            catch (Exception e)
            {
                Log.Error(this, "Error Enabling Subtask " + this.Name, e);
            }

            return false;
        }

        /// <summary>
        ///     Executes the subtask using the specified px node.
        /// </summary>
        /// <param name="pPxNode">The node.</param>
        /// <returns><c>true</c> if the success; otherwise false.</returns>
        public virtual bool Execute(IMMPxNode pPxNode)
        {
            try
            {
                return this.InternalExecute(pPxNode);
            }
            catch (Exception e)
            {
                Log.Error(this, "Error Executing Subtask " + this.Name, e);
            }

            return false;
        }

        /// <summary>
        ///     Initializes the subtask using the <paramref name="pPxApp" /> process framework application reference.
        /// </summary>
        /// <param name="pPxApp">The process framework application reference.</param>
        /// <returns>
        ///     <c>true</c> if intialized; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Initialize(IMMPxApplication pPxApp)
        {
            _PxApp = pPxApp;
            return true;
        }

        /// <summary>
        ///     Rollbacks the subtask using the specified px node.
        /// </summary>
        /// <param name="pPxNode">The node.</param>
        /// <returns><c>true</c> if rollback successfully; otherwise <c>false</c>.</returns>
        public virtual bool Rollback(IMMPxNode pPxNode)
        {
            try
            {
                return this.InternalRollback(pPxNode);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format("Error Rollback Subtask {0}", this.Name), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error(this, "Error Rollback Subtask " + this.Name, e);
            }

            return false;
        }

        #endregion

        #region IMMPxSubtask2 Members

        /// <summary>
        ///     Sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary Parameters
        {
            set { _Parameters = value; }
            protected get { return _Parameters; }
        }

        /// <summary>
        ///     Gets the supported extensions.
        /// </summary>
        /// <value>The supported extensions.</value>
        public IMMEnumExtensionNames SupportedExtensions
        {
            get { return _SupportedExtensions; }
        }

        /// <summary>
        ///     Gets the supported parameter names.
        /// </summary>
        /// <value>The supported parameter names.</value>
        public IDictionary SupportedParameterNames
        {
            get { return _SupportedParameters; }
        }

        /// <summary>
        ///     Sets the task.
        /// </summary>
        /// <value>
        ///     The task.
        /// </value>
        public IMMPxTask Task { protected get; set; }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            MMPxSubtasks.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMPxSubtasks.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the extension to the supported extensions.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        protected void AddExtension(string extensionName)
        {
            if (_SupportedExtensions == null)
                _SupportedExtensions = new PxExtensionNamesClass();

            _SupportedExtensions.Add(extensionName);
        }

        /// <summary>
        ///     Adds a parameter to the supported parameters.
        /// </summary>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="description">The description of the parameter.</param>
        protected void AddParameter(string key, string description)
        {
            if (_SupportedParameters == null)
                _SupportedParameters = new MMPxNodeListClass();

            object k = key;
            object d = description;
            _SupportedParameters.Add(ref k, ref d);
        }

        /// <summary>
        ///     Gets the value for the configuration at the application level.
        /// </summary>
        /// <param name="configName">Name of the config.</param>
        /// <returns>Returns a <see cref="string" /> representing the configuration parameter.</returns>
        protected string GetConfiguration(string configName)
        {
            IMMPxHelper2 helper = (IMMPxHelper2) _PxApp.Helper;
            return helper.GetConfigValue(configName);
        }

        /// <summary>
        ///     Gets the value for the configuration parameter at the subtask level.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        ///     Returns a value representing the configuration parameter.
        /// </returns>
        protected TValue GetParameter<TValue>(string key, TValue defaultValue)
        {
            return TypeCast.Cast(this.GetParameter(key), defaultValue);
        }

        /// <summary>
        ///     Gets the value for the configuration parameter at the subtask level.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the configuration parameter.</returns>
        protected string GetParameter(string key)
        {
            object objKey = key;
            string val = null;

            if (_Parameters.Exists(ref objKey))
            {
                object obj = _Parameters.get_Item(ref objKey);
                if (obj != null)
                    val = obj.ToString();
            }

            return val;
        }

        /// <summary>
        ///     Determines if the subtask should be enabled for the specified <see cref="IMMPxNode" />.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     <c>true</c> if the Subtask should be enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     This method will be called from IMMPxSubtask::Enabled
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract bool InternalEnabled(IMMPxNode node);

        /// <summary>
        ///     Executes the subtask using the specified <see cref="IMMPxNode" />.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     <c>true</c> if executes successfully; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     This method will be called from IMMPxSubtask::Execute
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract bool InternalExecute(IMMPxNode node);

        /// <summary>
        ///     Rollbacks the subtask using the specified px node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     <c>true</c> if rollback was successful; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalRollback(IMMPxNode node)
        {
            return true;
        }

        #endregion
    }
}