using System.Diagnostics;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Wraps the product <see cref="IMMWMSWorklocation" /> interface into an workable object.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ID = {ID}")]
    public class WorkLocation : BasePxNode<IMMWorkflowManager>
    {
        #region Constants

        /// <summary>
        ///     The name of the node type.
        /// </summary>
        internal const string NodeTypeName = "WorkLocation";

        #endregion

        #region Fields

        private Design _Design;
        private IMMWMSWorklocation _Worklocation;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxNode{TFrameworkExtension}" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="nodeId">The node identifier.</param>
        public WorkLocation(IMMPxApplication pxApp, int nodeId)
            : base(pxApp, NodeTypeName, nodeId)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxNode{TFrameworkExtension}" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        public WorkLocation(IMMPxApplication pxApp)
            : base(pxApp, NodeTypeName)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description
        {
            get { return _Worklocation.get_Description(); }
            set { _Worklocation.set_Description(ref value); }
        }

        /// <summary>
        ///     Gets or sets the design.
        /// </summary>
        /// <value>
        ///     The design.
        /// </value>
        public Design Design
        {
            get
            {
                if (_Design == null || !_Design.Valid)
                    _Design = new Design(PxApplication, _Worklocation.get_DesignID());

                return _Design;
            }
            set
            {
                int designId = value.ID;

                _Design = value;
                _Worklocation.set_DesignID(ref designId);
            }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        public override int ID
        {
            get { return _Worklocation.ID; }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BasePxNode{TFrameworkExtension}" /> is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen
        {
            get { return this.Design.IsOpen; }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name
        {
            get { return _Worklocation.get_Number(); }
            set { _Worklocation.set_Number(ref value); }
        }

        /// <summary>
        ///     Gets the site conditions.
        /// </summary>
        /// <value>
        ///     The site conditions.
        /// </value>
        public IMMWMSSiteConditions SiteConditions
        {
            get { return _Worklocation.SiteConditions; }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BasePxNode{TFrameworkExtension}" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public override bool Valid
        {
            get { return _Worklocation != null; }
        }

        #endregion        

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_Design != null)
            {
                _Design.Dispose();
                _Design = null;
            }
        }

        /// <summary>
        ///     Gets the process framework extension.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="T:TFrameworkExtension" /> representing the framework extension used for the node.
        /// </returns>
        protected override IMMWorkflowManager GetFrameworkExtension()
        {
            return base.PxApplication.GetWorkflowManager();
        }

        /// <summary>
        ///     Gets the list builder for the node.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="IMMListBuilder" /> representing the builder for the node.
        /// </returns>
        protected override IMMListBuilder GetListBuilder()
        {
            return null;
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="nodeId" /> for the node.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        protected override bool Initialize(IMMWorkflowManager extension, int nodeId)
        {
            if (_Worklocation != null && _Worklocation.ID == nodeId)
                return true;

            bool ro = false;
            bool sm = true;
            string nodeTypeName = NodeTypeName;

            _Worklocation = (IMMWMSWorklocation) extension.GetWMSNode(ref nodeTypeName, ref nodeId, ref ro, ref sm);

            return (_Worklocation != null);
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="user" /> for the node.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="user"></param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        protected override bool Initialize(IMMWorkflowManager extension, IMMPxUser user)
        {
            string nodeTypeName = NodeTypeName;

            _Worklocation = (IMMWMSWorklocation) extension.CreateWMSNode(ref nodeTypeName);

            return (_Worklocation != null);
        }

        #endregion
    }
}