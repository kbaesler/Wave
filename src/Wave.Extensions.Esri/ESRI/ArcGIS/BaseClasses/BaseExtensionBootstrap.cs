using System;
using System.Collections.Generic;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    /// Provides a strongly-typed service container.
    /// </summary>
    public interface IBootstrapContainer
    {
        #region Public Methods

        /// <summary>
        ///     Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">
        ///     An instance of the service type to add. This object must implement or inherit from the
        ///     type indicated by the <paramref name="serviceType" /> parameter.
        /// </param>
        void AddService(Type serviceType, object serviceInstance);

        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="TServiceType">The type of the service type.</typeparam>
        /// <returns>
        ///     A service object of type <typeparamref name="TServiceType" />
        ///     -or-
        ///     null if there is no service object of type <typeparamref name="TServiceType" />.
        /// </returns>
        TServiceType GetService<TServiceType>();

        #endregion
    }

    /// <summary>
    ///     An extension that implements the inversion of control design pattern.
    /// </summary>
    public abstract class BaseExtensionBootstrap : BaseExtension, IBootstrapContainer
    {
        #region Fields

        private readonly Dictionary<Type, object> _ServiceContainer = new Dictionary<Type, object>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseExtensionBootstrap" /> class.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        protected BaseExtensionBootstrap(string extensionName)
            : base(extensionName)
        {
        }

        #endregion

        #region IBootstrapContainer Members

        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="TServiceType">The type of the service type.</typeparam>
        /// <returns>
        ///     A service object of type <typeparamref name="TServiceType" />
        ///     -or-
        ///     null if there is no service object of type <typeparamref name="TServiceType" />.
        /// </returns>
        public TServiceType GetService<TServiceType>()
        {
            Type type = typeof (TServiceType);

            if (_ServiceContainer.ContainsKey(type))
                return (TServiceType) _ServiceContainer[type];

            return default(TServiceType);
        }

        /// <summary>
        ///     Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">
        ///     An instance of the service type to add. This object must implement or inherit from the
        ///     type indicated by the <paramref name="serviceType" /> parameter.
        /// </param>
        public void AddService(Type serviceType, object serviceInstance)
        {
            if (!_ServiceContainer.ContainsKey(serviceType))
                _ServiceContainer.Add(serviceType, serviceInstance);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public override void Startup(ref object initializationData)
        {
            base.Startup(ref initializationData);

            this.Configure();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Configures the bootstrap extension.
        /// </summary>
        protected abstract void Configure();

        #endregion
    }
}