using System.Diagnostics.CodeAnalysis;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract class used to handle creating a custom cost report within Workflow Manager.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Px")]
    public abstract class BasePxCostEngine : BasePxReportingEngine
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxCostEngine" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BasePxCostEngine(string name)
            : base(name, "CostXSLPath")
        {
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
        /// <remarks>
        ///     By default it will return the XML string for the ArcFM Cost Engine.
        /// </remarks>
        protected override string InternalExecute(IMMPxNode node)
        {
            IMMWMSReportingEngine engine = new clsMMCostEngineClass();
            if (engine.Initialize(this.PxApplication))
                return engine.Execute(node);

            return string.Empty;
        }

        #endregion
    }
}