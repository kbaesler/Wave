using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework.Search;
using Miner.FrameworkUI.Search;
using Miner.Interop;

namespace Miner.Framework.BaseClasses
{
    /// <summary>
    ///     An abstract class that is used to display the user interface for the locator strategies.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseSearchStrategyUI : IMMSearchStrategyUI
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseSearchStrategyUI" /> class.
        /// </summary>
        /// <param name="caption">
        ///     The name of the search strategy as it appears in the Locator tool pulldown menu (e.g., Attribute
        ///     Query).
        /// </param>
        /// <param name="priority">The order in which the locators appear in the dropdown menu.</param>
        /// <param name="searchStrategy">The search strategy that contains the Find method that executes the search.</param>
        /// <param name="control">The user control.</param>
        protected BaseSearchStrategyUI(string caption, int priority, IMMSearchStrategy searchStrategy,
            UserControl control)
        {
            this.Caption = caption;
            this.Priority = priority;
            this.SearchStrategy = searchStrategy;
            this.Control = control;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the control.
        /// </summary>
        /// <value>
        ///     The control.
        /// </value>
        protected UserControl Control { get; private set; }

        #endregion

        #region IMMSearchStrategyUI Members

        /// <summary>
        ///     Gets the caption.
        /// </summary>
        /// <value>
        ///     The caption.
        /// </value>
        /// <remarks>
        ///     This property contains the name of the search strategy as it appears in the Locator tool pulldown menu (e.g.,
        ///     Attribute Query).
        /// </remarks>
        public string Caption { get; protected set; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <value>
        ///     The priority.
        /// </value>
        /// <remarks>
        ///     Allows you to set the order in which the locators appear in the dropdown menu.
        /// </remarks>
        public int Priority { get; protected set; }

        /// <summary>
        ///     Gets the search strategy.
        /// </summary>
        /// <value>
        ///     The search strategy.
        /// </value>
        /// <remarks>
        ///     This property takes an <see cref="Miner.Interop.IMMSearchStrategy" /> object. This object contains the Find method
        ///     that executes the search.
        /// </remarks>
        public IMMSearchStrategy SearchStrategy { get; protected set; }

        /// <summary>
        ///     Gets the COM prog ID.
        /// </summary>
        /// <remarks>
        ///     If the search strategy has been written in VB6, this property requires its ProgID.
        ///     If the search strategy has been written in C#, this returns an empty string.
        /// </remarks>
        public virtual string COMProgID
        {
            get { return string.Empty; }
        }

        /// <summary>
        ///     Gets the results processor.
        /// </summary>
        /// <value>
        ///     The results processor.
        /// </value>
        /// <remarks>
        ///     This optional property returns an <see cref="Miner.Interop.IMMResultsProcessor" /> object.
        ///     This object allows you to determine how the objects in the tree appear (e.g., grouped). Enter a value of Null to
        ///     use the standard format as it is displayed in the Attribute Editor.
        ///     While in Feeder Manager mode, the results processor for Feeder Manager is always used.
        /// </remarks>
        public virtual IMMResultsProcessor ResultsProcessor
        {
            get { return new StandardResultsProcessor(); }
        }

        /// <summary>
        ///     Uses the map and an object class to initialize the search strategy.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="classFilter">The class filter.</param>
        /// <remarks>
        ///     It may be called several times during the lifetime of the application depending on changes to the map and map
        ///     layers.
        ///     This method is called each time items are added or removed from the map.
        /// </remarks>
        public void InitializeStrategyUI(IMap map, IObjectClass classFilter)
        {
            try
            {
                this.InitializeComponent(this.Control, map, classFilter);
            }
            catch (Exception e)
            {
                Log.Error(this, this.Caption, e);
            }
        }

        /// <summary>
        ///     Notifies a current open locator that the user has selected another locator and provides the opportunity to perform
        ///     any necessary state changes
        /// </summary>
        public virtual void Deactivated()
        {
        }

        /// <summary>
        ///     Returns an <see cref="Miner.Interop.IMMSearchConfiguration" /> object that contains the
        ///     <see cref="ESRI.ArcGIS.esriSystem.IPropertySet" /> specific to the search being conducted.
        /// </summary>
        /// <param name="optionFlags">The option flags.</param>
        /// <returns>
        ///     Returns the <see cref="Miner.Interop.IMMSearchConfiguration" /> that contains the
        ///     <see cref="ESRI.ArcGIS.esriSystem.IPropertySet" /> specific to the search.
        /// </returns>
        public virtual IMMSearchConfiguration GetSearchConfiguration(mmSearchOptionFlags optionFlags)
        {
            IMMSearchConfiguration config = new SearchConfiguration();
            config.SearchParameters = this.GetSearchParameters(optionFlags);
            return config;
        }

        /// <summary>
        ///     Clears the user interface in preparation for the next search.
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        ///     Cleans up in preparation for shutting down the application.
        /// </summary>
        public virtual void Shutdown()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the search parameters for the <see cref="Miner.Interop.IMMSearchStrategy" />
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        /// <returns>
        ///     The search parameters that are passed into the <see cref="Miner.Interop.IMMSearchStrategy" />.
        /// </returns>
        protected virtual object GetSearchParameters(mmSearchOptionFlags searchOptions)
        {
            return null;
        }

        /// <summary>
        ///     Uses the map and an object class to initialize the search strategy.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="map">The map.</param>
        /// <param name="classFilter">The class filter.</param>
        /// <remarks>
        ///     It may be called several times during the lifetime of the application depending on changes to the map and map
        ///     layers.
        ///     This method is called each time items are added or removed from the map.
        /// </remarks>
        protected abstract void InitializeComponent(UserControl control, IMap map, IObjectClass classFilter);

        #endregion
    }
}