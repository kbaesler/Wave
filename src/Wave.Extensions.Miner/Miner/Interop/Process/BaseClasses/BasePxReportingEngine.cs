using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract class used to create a custom report for either the cost engine or inventory within Workflow Manager.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Px")]
    public abstract class BasePxReportingEngine : IMMWMSReportingEngine, IMMPxDisplayName
    {
        private static readonly ILog Log = LogProvider.For<BasePxReportingEngine>();

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxReportingEngine" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="styleSheetName">Name of the style sheet.</param>
        protected BasePxReportingEngine(string displayName, string styleSheetName)
        {
            this.DisplayName = displayName;
            this.StyleSheetName = styleSheetName;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the px application.
        /// </summary>
        /// <value>
        ///     The px application.
        /// </value>
        protected IMMPxApplication PxApplication { get; set; }

        /// <summary>
        ///     Gets the name of the style sheet.
        /// </summary>
        /// <value>
        ///     The name of the style sheet.
        /// </value>
        protected string StyleSheetName { get; private set; }

        /// <summary>
        ///     Gets the path to the XSL file that is used to translate the XML into HTML.
        /// </summary>
        protected virtual string XslPath
        {
            get
            {
                IMMRegistry registry = new MMRegistry();
                registry.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmWMS, "Style Sheets");
                return TypeCast.Cast(registry.Read(this.StyleSheetName, string.Empty), string.Empty);
            }
        }

        #endregion

        #region IMMPxDisplayName Members

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName { get; protected set; }

        #endregion

        #region IMMWMSReportingEngine Members

        /// <summary>
        ///     Executes the cost engine for the specified report.
        /// </summary>
        /// <param name="pPxNode">The node.</param>
        /// <returns>
        ///     An XML <see cref="string" /> of the report.
        /// </returns>
        public virtual string Execute(IMMPxNode pPxNode)
        {
            try
            {
                return this.InternalExecute(pPxNode);
            }
            catch (Exception e)
            {
                Log.Error("Error Executing Reporting Engine " + this.DisplayName, e);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Initializes the <see cref="BasePxReportingEngine" />.
        /// </summary>
        /// <param name="pPxApp">The Px Application reference.</param>
        /// <returns>
        ///     <c>true</c> if the class has been initalized, otherwise <c>false</c>.
        /// </returns>
        public virtual bool Initialize(IMMPxApplication pPxApp)
        {
            try
            {
                return this.InternalInitialize(pPxApp);
            }
            catch (Exception e)
            {
                Log.Error("Error Initializing Reporting Engine " + this.DisplayName, e);
            }

            return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Executes the cost engine for the specified report.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     An XML <see cref="string" /> of the report.
        /// </returns>
        protected virtual string InternalExecute(IMMPxNode node)
        {
            return string.Empty;
        }

        /// <summary>
        ///     Initializes the <see cref="BasePxReportingEngine" />.
        /// </summary>
        /// <param name="pxApp">The Px Application reference.</param>
        /// <returns>
        ///     <c>true</c> if the class has been initalized, otherwise <c>false</c>.
        /// </returns>
        protected virtual bool InternalInitialize(IMMPxApplication pxApp)
        {
            this.PxApplication = pxApp;

            return (pxApp != null);
        }

        #endregion
    }
}