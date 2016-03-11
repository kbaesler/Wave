using System.Diagnostics;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Wraps the product <see cref="IMMWMSCompatibleUnit" /> interface into an workable object.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ID = {ID}")]
    public class CompatibleUnit : BasePxNode<IMMWorkflowManager>
    {
        #region Constants

        /// <summary>
        ///     The name of the node type.
        /// </summary>
        internal const string NodeTypeName = "CU";

        #endregion

        #region Fields

        private IMMWMSCompatibleUnit _CompatibleUnit;
        private Design _Design;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxNode{TFrameworkExtension}" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="nodeId">The node identifier.</param>
        public CompatibleUnit(IMMPxApplication pxApp, int nodeId)
            : base(pxApp, NodeTypeName, nodeId)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxNode{TFrameworkExtension}" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        public CompatibleUnit(IMMPxApplication pxApp)
            : base(pxApp, NodeTypeName)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the site conditions.
        /// </summary>
        /// <value>
        /// The site conditions.
        /// </value>
        public IMMWMSSiteConditions SiteConditions
        {
            get { return _CompatibleUnit.SiteConditions; }
        }
        /// <summary>
        ///     Gets or sets the code.
        /// </summary>
        /// <value>
        ///     The code.
        /// </value>
        public string Code
        {
            get { return _CompatibleUnit.get_CuCode(); }
            set { _CompatibleUnit.set_CuCode(ref value); }
        }

        /// <summary>
        /// Gets or sets the design.
        /// </summary>
        /// <value>
        /// The design.
        /// </value>
        public Design Design
        {
            get
            {
                if (_Design == null || !_Design.Valid)
                    _Design = new Design(PxApplication, _CompatibleUnit.get_DesignID());

                return _Design;
            }
            set
            {
                int designId = value.ID;

                _Design = value;
                _CompatibleUnit.set_DesignID(ref designId);                
            }
        }        
        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description
        {
            get { return _CompatibleUnit.Description; }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        public override int ID
        {
            get { return _CompatibleUnit.ID; }
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
        ///     Gets or sets the library identifier.
        /// </summary>
        /// <value>
        ///     The library identifier.
        /// </value>
        public int LibraryID
        {
            get { return _CompatibleUnit.get_CuLibID(); }
            set { _CompatibleUnit.set_CuLibID(ref value); }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name
        {
            get { return _CompatibleUnit.get_CuName(); }
            set { _CompatibleUnit.set_CuName(ref value); }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BasePxNode{TFrameworkExtension}" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public override bool Valid
        {
            get { return _CompatibleUnit != null; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Deletes the node from the process framework database.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            if (_Design != null)
            {
                _Design.Dispose();
                _Design = null;
            }
        }

        /// <summary>
        ///     Updates the node by flushing the information to the database and reinitializing the underlying
        ///     <see cref="Miner.Interop.Process.IMMPxNode" />.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (_Design != null)
            {
                _Design.Dispose();
                _Design = null;
            }
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
            if (_CompatibleUnit != null && _CompatibleUnit.ID == nodeId)
                return true;

            bool ro = false;
            bool sm = true;
            string nodeTypeName = NodeTypeName;

            _CompatibleUnit = (IMMWMSCompatibleUnit) extension.GetWMSNode(ref nodeTypeName, ref nodeId, ref ro, ref sm);

            return (_CompatibleUnit != null);
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="nodeId" /> for the node.
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

            _CompatibleUnit = (IMMWMSCompatibleUnit) extension.CreateWMSNode(ref nodeTypeName);

            return (_CompatibleUnit != null);
        }

        #endregion
    }
}