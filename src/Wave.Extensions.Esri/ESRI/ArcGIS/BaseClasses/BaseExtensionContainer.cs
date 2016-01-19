using System;
using System.Collections.Generic;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     Provides a strongly-typed service container.
    /// </summary>
    public interface IExtensionContainer
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
    public abstract class BaseExtensionContainer : BaseExtension, IExtensionContainer
    {
        #region Fields

        private readonly Dictionary<Type, object> _ExtensionContainer = new Dictionary<Type, object>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseExtensionContainer" /> class.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        protected BaseExtensionContainer(string extensionName)
            : base(extensionName)
        {
        }

        #endregion

        #region IExtensionContainer Members

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

            if (_ExtensionContainer.ContainsKey(type))
                return (TServiceType) _ExtensionContainer[type];

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
            if (!_ExtensionContainer.ContainsKey(serviceType))
                _ExtensionContainer.Add(serviceType, serviceInstance);
        }

        #endregion
    }
}