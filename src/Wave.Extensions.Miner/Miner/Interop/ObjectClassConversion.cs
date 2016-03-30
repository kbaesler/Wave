using System;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides a static singleton for the <see cref="IMMObjectClassConversionTool" /> instance that is used to convert
    ///     object classes to ArcFM, Designer or ESRI objects.
    /// </summary>
    public static class ObjectClassConversion
    {
        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IMMObjectClassConversionTool Instance
        {
            get
            {
                Type t = Type.GetTypeFromProgID(ArcFM.Commands.Name.ObjectClassConversion);
                var conversion = Activator.CreateInstance(t) as IMMObjectClassConversionTool;
                return conversion;
            }
        }

        #endregion
    }
}