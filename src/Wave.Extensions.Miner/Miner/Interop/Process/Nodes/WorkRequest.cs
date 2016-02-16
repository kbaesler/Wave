using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Wraps the product <see cref="Miner.Interop.Process.IMMWMSWorkRequest" /> interface into an workable object.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ID = {ID}")]
    public class WorkRequest : BasePxNode<IMMWorkflowManager>, IPxWorkRequest
    {
        #region Constants

        /// <summary>
        ///     The name of the node type.
        /// </summary>
        internal const string NodeTypeName = "WorkRequest";

        #endregion

        #region Fields

        private Customer _Customer;
        private Location _Location;
        private IMMWMSWorkRequest _WorkRequest;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkRequest" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        public WorkRequest(IMMPxApplication pxApp)
            : base(pxApp, NodeTypeName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkRequest" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="workRequest">The work request.</param>
        public WorkRequest(IMMPxApplication pxApp, IMMWMSWorkRequest workRequest)
            : base(pxApp, NodeTypeName, workRequest.ID)
        {
            _WorkRequest = workRequest;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkRequest" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="nodeId">The node identifier.</param>
        public WorkRequest(IMMPxApplication pxApp, int nodeId)
            : base(pxApp, NodeTypeName, nodeId)
        {
        }

        #endregion

        #region IPxWorkRequest Members

        /// <summary>
        ///     Get or set the Comments associated with this instance.
        /// </summary>
        /// <value>
        ///     The comments.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The comments cannot be larger then 255 characters.</exception>
        public string Comments
        {
            get
            {
                IMMWMSPropertySet propset = _WorkRequest.PropertySet;
                return propset.GetValue("COMMENTS", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _WorkRequest.PropertySet;
                if (propset.SetValue("COMMENTS", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the customer.
        /// </summary>
        /// <value>
        ///     The customer.
        /// </value>
        public Customer Customer
        {
            get
            {
                if (_Customer == null)
                {
                    if (_WorkRequest != null && _WorkRequest.Customer != null)
                        _Customer = new Customer(_WorkRequest.Customer);
                }

                return _Customer;
            }
        }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The description cannot be larger then 128 characters.</exception>
        public string Description
        {
            get { return _WorkRequest.get_Description(); }
            set
            {
                if (value != null && value.Length > 32)
                {
                    throw new ArgumentOutOfRangeException("value", @"The description cannot be larger then 128 characters.");
                }

                _WorkRequest.set_Description(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets all of the designs that are associated with the <see cref="IPxWorkRequest" />.
        /// </summary>
        public IEnumerable<IPxDesign> Designs
        {
            get
            {
                if (!this.Valid)
                    yield return null;

                string commandText = string.Format("SELECT ID FROM {0} WHERE WORK_REQUEST_ID = {1}",
                    base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.Design), this.ID);

                DataTable table = base.PxApplication.ExecuteQuery(commandText);
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Design design = new Design(base.PxApplication, row.Field<int>(0));
                        yield return design;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the end date.
        /// </summary>
        /// <value>
        ///     The end date.
        /// </value>
        public DateTime? EndDate
        {
            get
            {
                IMMWMSPropertySet propset = _WorkRequest.PropertySet;
                return propset.GetValue("END_DATE", default(DateTime?));
            }
            set
            {
                IMMWMSPropertySet propset = _WorkRequest.PropertySet;
                if (propset.SetValue("END_DATE", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the location.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        public Location Location
        {
            get
            {
                if (_Location == null)
                {
                    if (_WorkRequest != null && _WorkRequest.Location != null)
                        _Location = new Location(_WorkRequest.Location);
                }

                return _Location;
            }
        }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public IMMPxUser Owner
        {
            get { return base.PxApplication.GetUser(_WorkRequest.get_OwnerID()); }
            set
            {
                int ownerID = value.Id;
                _WorkRequest.set_OwnerID(ref ownerID);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        /// <value>
        ///     The start date.
        /// </value>
        public DateTime? StartDate
        {
            get
            {
                IMMWMSPropertySet propset = _WorkRequest.PropertySet;
                return propset.GetValue("START_DATE", default(DateTime?));
            }
            set
            {
                IMMWMSPropertySet propset = _WorkRequest.PropertySet;
                if (propset.SetValue("START_DATE", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        public override int ID
        {
            get { return (_WorkRequest != null) ? _WorkRequest.ID : -1; }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="WorkRequest" /> is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="WorkRequest" /> is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen
        {
            get
            {
                WorkRequest request = base.PxApplication.GetWorkRequest();
                if (request == null) return false;

                bool isOpen = (request.ID == this.ID);
                return isOpen;
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <exception cref="ArgumentOutOfRangeException">The name cannot be larger then 64 characters.</exception>
        public override string Name
        {
            get { return _WorkRequest.get_Name(); }
            set
            {
                if (value != null && value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("value", @"The name cannot be larger then 64 characters.");
                }

                _WorkRequest.set_Name(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="WorkRequest" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public override bool Valid
        {
            get { return (_WorkRequest != null); }
        }

        /// <summary>
        ///     Deletes the node from the process framework database.
        /// </summary>
        public override void Delete()
        {
            // Delete the node.
            base.Delete();

            // Remove the references.
            this.Dispose(true);
        }

        /// <summary>
        ///     Updates the node by flushing the information to the database and reinitializing the underlying
        ///     <see cref="Miner.Interop.Process.IMMPxNode" />.
        /// </summary>
        public override void Update()
        {
            if (_WorkRequest != null)
            {
                // Update the information.
                ((IMMWMSNode) _WorkRequest).UpdateDB();

                // Call the base implementation.
                base.Update();
            }

            if (_Location != null)
            {
                _Location.Update();
            }

            if (_Customer != null)
            {
                _Customer.Update();
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
            if (disposing)
            {
                if (_WorkRequest != null)
                    while (Marshal.ReleaseComObject(_WorkRequest) > 0)
                    {
                    }

                if (_Location != null)
                    _Location.Dispose();

                if (_Customer != null)
                    _Customer.Dispose();

                _WorkRequest = null;
                _Location = null;
                _Customer = null;
            }

            base.Dispose(true);
        }

        /// <summary>
        ///     Gets the process framework extension.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="IMMWorkflowManager" /> representing the framework extension used for the node.
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
            IMMPxListBuilderInit builder = new clsWRBuilderClass();
            builder.PxApplication = base.PxApplication;

            return (IMMListBuilder) builder;
        }

        /// <summary>
        ///     Creates the process framework node wrapper for the specified the <paramref name="user" />.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="user">The current user.</param>
        protected override bool Initialize(IMMWorkflowManager extension, IMMPxUser user)
        {
            int ownerID = user.Id;
            string nodeTypeName = NodeTypeName;

            _WorkRequest = (IMMWMSWorkRequest) extension.CreateWMSNode(ref nodeTypeName);
            _WorkRequest.set_OwnerID(ref ownerID);

            return (_WorkRequest != null);
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="nodeID" /> for the node.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="nodeID">The node ID.</param>
        /// <returns>
        ///     Returns <see cref="Boolean" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        protected override bool Initialize(IMMWorkflowManager extension, int nodeID)
        {
            // Verify that the existing session isn't the same node.
            if (_WorkRequest != null && _WorkRequest.ID == nodeID)
                return true;

            bool ro = false;
            bool sm = true;
            string nodeTypeName = NodeTypeName;

            _WorkRequest = (IMMWMSWorkRequest) extension.GetWMSNode(ref nodeTypeName, ref nodeID, ref ro, ref sm);

            return (_WorkRequest != null);
        }

        #endregion
    }
}