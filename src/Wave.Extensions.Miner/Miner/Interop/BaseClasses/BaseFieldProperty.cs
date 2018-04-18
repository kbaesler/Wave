using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Miner.Interop
{
    /// <summary>
    ///     An abstract <see cref="IMMFieldProperty" /> that are used to control the behavior of fields in ArcFM.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseFieldProperty : IMMFieldProperty
    {
        #region Fields

        private static readonly ILog Log = LogProvider.For<BaseFieldProperty>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseFieldProperty" /> class.
        /// </summary>
        /// <param name="name">The display name of the field property.  </param>
        protected BaseFieldProperty(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the display name of the field property.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines if the field manager contains the properties for the field strategy.
        /// </summary>
        /// <param name="pFieldManager">The field manager.</param>
        /// <returns>
        ///     Any non-zero value returned here will equal true. This method is treated as boolean.
        /// </returns>
        public int Value(IMMFieldManager pFieldManager)
        {
            try
            {
                var valid = this.IsValid(pFieldManager);
                return valid ? 1 : 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return 0;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction()]
        internal static void Register(string regKey)
        {
            FieldPropertyStrategy.Register(regKey);
        }

        /// <summary>
        ///     Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction()]
        internal static void Unregister(string regKey)
        {
            FieldPropertyStrategy.Unregister(regKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Determines if the field manager contains the valid properties for the field strategy.
        /// </summary>
        /// <param name="fieldManager">The field manager.</param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when valid; otherwise <c>false</c></returns>
        protected abstract bool IsValid(IMMFieldManager fieldManager);

        #endregion
    }
}