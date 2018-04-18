using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides autotext elements with access to text strings which are updated in various ways and at various times
    ///     depending on the auto text source object.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseAutoText : IMMAutoTextSource
    {
        #region Fields

        private static readonly ILog Log = LogProvider.For<BaseAutoText>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseAutoText" /> class.
        /// </summary>
        /// <param name="caption">This is the autotext element's display name. The name will appear in the ArcFM Autotext menu.</param>
        protected BaseAutoText(string caption)
        {
            this.Caption = caption;
            this.ProgID = this.GetType().GetCustomAttributes(typeof(ProgIdAttribute), true).Cast<ProgIdAttribute>().Select(o => o.Value).FirstOrDefault();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the caption.
        /// </summary>
        /// <remarks>This is the autotext element's display name. The name will appear in the ArcFM Autotext menu.  </remarks>
        public string Caption { get; private set; }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        /// <remarks>This property isn't currently used by ArcFM. </remarks>
        public string Message { get; set; }

        /// <summary>
        ///     Gets the program ID.
        /// </summary>
        /// <remarks>
        ///     The program ID links the autotext element with its text source class.
        ///     The program ID is from the class module that implements this interface.
        /// </remarks>
        public virtual string ProgID { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a boolean value indicating whether the element should be refreshed based on the
        ///     <paramref name="eTextEvent" /> value.
        /// </summary>
        /// <param name="eTextEvent">The text event.</param>
        /// <returns>
        ///     Returns a boolean value indicating whether the element should be refreshed based on the mmAutoTextEvents value.
        /// </returns>
        public virtual bool NeedRefresh(mmAutoTextEvents eTextEvent)
        {
            return true;
        }

        /// <summary>
        ///     Returns the text string that will appear on the map layout based on the <paramref name="eTextEvent" /> value
        ///     and the status of the <paramref name="pMapProdInfo" /> parameter.
        ///     The <paramref name="pMapProdInfo" /> parameter passed in will be "Nothing" for all <paramref name="eTextEvent" />
        ///     except mmPlotNewPage and mmFinishPlot.
        /// </summary>
        /// <param name="eTextEvent">The text event.</param>
        /// <param name="pMapProdInfo">The map production information.</param>
        /// <returns></returns>
        /// Returns the text string that will appear on the map layout based on the
        /// <paramref name="eTextEvent" />
        /// value
        /// and the status of the
        /// <paramref name="pMapProdInfo" />
        /// parameter.
        /// <remarks>
        ///     This method should always return a non-empty string. If nothing is provided, it is automatically set to " ".
        /// </remarks>
        public string TextString(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            var value = " ";

            try
            {
                switch (eTextEvent)
                {
                    case mmAutoTextEvents.mmCreate:
                        value = this.OnCreate();
                        break;

                    case mmAutoTextEvents.mmDraw:
                        value = this.OnDraw();
                        break;

                    case mmAutoTextEvents.mmFinishPlot:
                        value = this.OnFinish();
                        break;

                    case mmAutoTextEvents.mmPlotNewPage:
                        value = this.GetText(pMapProdInfo);
                        break;

                    case mmAutoTextEvents.mmPrint:
                        value = this.OnPrint(pMapProdInfo);
                        break;

                    case mmAutoTextEvents.mmRefresh:
                        value = this.OnRefresh();
                        break;

                    default:
                        value = this.OnStart(pMapProdInfo);
                        break;
                }
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format("Error Executing Auto Text {0}", this.Caption), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error("Error Executing Auto Text " + this.Caption, e);
            }

            // An empty string will remove the auto text element.
            return string.IsNullOrEmpty(value) ? " " : value;
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
            MMCustomTextSources.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMCustomTextSources.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Returns the text string that will appear on the map layout base on
        ///     the status of the <paramref name="mapProdInfo" /> parameter.
        /// </summary>
        /// <param name="mapProdInfo">The map prod info.</param>
        /// <returns></returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        /// Returns the text string that will appear on the map layout based on the status of the
        /// <paramref name="mapProdInfo" />
        /// parameter.
        protected abstract string GetText(IMMMapProductionInfo mapProdInfo);

        /// <summary>
        ///     Called when the auto text has been created.
        /// </summary>
        /// <returns>Returns the text string that will appear on the map layout</returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        protected virtual string OnCreate()
        {
            return this.Caption;
        }

        /// <summary>
        ///     Called when the auto text has been drawn.
        /// </summary>
        /// <returns>Returns the text string that will appear on the map layout</returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        protected virtual string OnDraw()
        {
            return this.Caption;
        }

        /// <summary>
        ///     Called when the auto text has finished being plotted.
        /// </summary>
        /// <returns>Returns the text string that will appear on the map layout</returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        protected virtual string OnFinish()
        {
            return this.Caption;
        }

        /// <summary>
        ///     Called when the page layout is being printed.
        /// </summary>
        /// <param name="mapProdInfo">The map product information.</param>
        /// <returns>
        ///     Returns the text string that will appear on the map layout
        /// </returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        protected virtual string OnPrint(IMMMapProductionInfo mapProdInfo)
        {
            return this.GetText(mapProdInfo);
        }

        /// <summary>
        ///     Called when the auto text has been refreshed.
        /// </summary>
        /// <returns>
        ///     Returns the text string that will appear on the map layout
        /// </returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        protected virtual string OnRefresh()
        {
            return this.Caption;
        }

        /// <summary>
        ///     Called when the page is starting to be plotted.
        /// </summary>
        /// <param name="mapProdInfo">The map product information.</param>
        /// <returns>
        ///     Returns the text string that will appear on the map layout
        /// </returns>
        /// <remarks>
        ///     This method should always return a non-empty string.
        /// </remarks>
        protected virtual string OnStart(IMMMapProductionInfo mapProdInfo)
        {
            return this.GetText(mapProdInfo);
        }

        #endregion
    }
}