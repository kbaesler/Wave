using System;
using System.Diagnostics;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides access to expose the progress events for the <see cref="IFeatureProgress" /> and
    ///     <see cref="IReplicaProgress" /> events.
    /// </summary>
    /// <remarks>
    ///     To 'wire' these event handlers into an event source, it is necessary to use the IConnectionPointContainer
    ///     interface, which is implemented by several classes,
    ///     including FeatureDataConverter (in the Geodatabase library) and CheckOut (in the Distributed Geodatabase library).
    ///     Once the correct connection point has been identified by GUID comparison, the connection point can be 'advised'
    ///     that an instance of the helper class is
    ///     going to handle events.
    /// </remarks>
    /// <seealso cref="ESRI.ArcGIS.Geodatabase.IFeatureProgress" />
    /// http://support.esri.com/technical-article/000010047
    public abstract class FeatureProgress : IFeatureProgress
    {
        #region Fields

        private static readonly ILog Log = LogProvider.For<FeatureProgress>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureProgress" /> class.
        /// </summary>
        protected FeatureProgress()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureProgress" /> class.
        /// </summary>
        /// <param name="connectionPointContainer">The connection point container.</param>
        /// <exception cref="ArgumentException">An feature progress connection point could not be found.</exception>
        protected FeatureProgress(IConnectionPointContainer connectionPointContainer)
        {
            this.Advise(connectionPointContainer);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count { get; private set; }

        /// <summary>
        ///     Sets the name of the feature class.
        /// </summary>
        /// <value>
        ///     The name of the feature class.
        /// </value>
        public virtual string FeatureClassName { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is cancelled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is cancelled; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsCancelled { get; set; }

        /// <summary>
        ///     Sets the maximum features.
        /// </summary>
        /// <value>
        ///     The maximum features.
        /// </value>
        public virtual int MaxFeatures { get; set; }

        /// <summary>
        ///     Sets the minimum features.
        /// </summary>
        /// <value>
        ///     The minimum features.
        /// </value>
        public virtual int MinFeatures { get; set; }

        /// <summary>
        ///     Sets the position.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        public virtual int Position { get; set; }

        /// <summary>
        ///     Sets the step value.
        /// </summary>
        /// <value>
        ///     The step value.
        /// </value>
        public virtual int StepValue { get; set; }

        #endregion

        #region IFeatureProgress Members

        /// <summary>
        ///     Steps this instance.
        /// </summary>
        public virtual void Step()
        {
            Count += StepValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Advises the specified connection point container.
        /// </summary>
        /// <param name="connectionPointContainer">The connection point container.</param>
        /// <exception cref="ArgumentException">An feature progress connection point could not be found.</exception>
        public void Advise(IConnectionPointContainer connectionPointContainer)
        {
            IEnumConnectionPoints enumConnectionPoints;
            connectionPointContainer.EnumConnectionPoints(out enumConnectionPoints);
            enumConnectionPoints.Reset();

            IConnectionPoint connectionPoint;
            Guid guid = typeof(IFeatureProgress).GUID;

            uint pcFetched;
            enumConnectionPoints.RemoteNext(1, out connectionPoint, out pcFetched);
            while (connectionPoint != null)
            {
                Guid connectionGuid;
                connectionPoint.GetConnectionInterface(out connectionGuid);

                if (connectionGuid == guid)
                {
                    break;
                }

                enumConnectionPoints.RemoteNext(1, out connectionPoint, out pcFetched);
            }

            if (connectionPoint == null)
            {
                throw new ArgumentException("An feature progress connection point could not be found.");
            }

            uint connectionPointCookie;
            connectionPoint.Advise(this, out connectionPointCookie);
        }

        #endregion

        #region Nested Type: Default

        public class Default : FeatureProgress
        {
            #region Public Properties

            public override string FeatureClassName
            {
                set { Log.Info("\tName: {0}", value); }
            }

            public override int MaxFeatures
            {
                set { Log.Info("\tMaximum: {0:N0}", value); }
            }

            public override int MinFeatures
            {
                set { Log.Info("\tMinimum: {0:N0}", value); }
            }

            public override int Position
            {
                set { Log.Info("\tPosition: {0:N0}", value); }
            }

            #endregion

            #region Public Methods

            public override void Step()
            {
                base.Step();

                Log.Info("\t\t{0:N0} row(s) exported.", Count);
            }

            #endregion
        }

        #endregion
    }
}