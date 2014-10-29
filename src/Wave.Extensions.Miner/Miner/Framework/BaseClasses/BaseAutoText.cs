using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Miner.Interop;

namespace Miner.Framework.BaseClasses
{
    /// <summary>
    ///     Provides autotext elements with access to text strings which are updated in various ways and at various times
    ///     depending on the auto text source object.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseAutoText : IMMAutoTextSource
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseAutoText" /> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="progID">The prog ID.</param>
        protected BaseAutoText(string caption, string progID)
        {
            this.Caption = caption;
            this.ProgID = progID;
        }

        #endregion

        #region IMMAutoTextSource Members

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
        public string ProgID { get; private set; }

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
            try
            {
                return this.GetText(eTextEvent, pMapProdInfo);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format("Error Executing Auto Text {0}", this.Caption), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error(this, "Error Executing Auto Text " + this.Caption, e);
            }

            return string.Empty;
        }

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

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Returns the text string that will appear on the map layout based on the <paramref name="textEvent" /> value
        ///     and the status of the <paramref name="mapProdInfo" /> parameter.
        ///     The <paramref name="mapProdInfo" /> parameter passed in will be "Nothing" for all <paramref name="textEvent" />
        ///     except mmPlotNewPage and mmFinishPlot.
        /// </summary>
        /// <param name="textEvent">The text event.</param>
        /// <param name="mapProdInfo">The map prod info.</param>
        /// Returns the text string that will appear on the map layout based on the
        /// <paramref name="textEvent" />
        /// value
        /// and the status of the
        /// <paramref name="mapProdInfo" />
        /// parameter.
        /// <remarks>This method should always return a non-empty string. If nothing is provided, it is automatically set to " ".</remarks>
        protected abstract string GetText(mmAutoTextEvents textEvent, IMMMapProductionInfo mapProdInfo);

        #endregion
    }
}