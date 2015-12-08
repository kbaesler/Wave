﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop.msxml2;

namespace Miner.Interop.Process
{
    /// <summary>
    /// Wraps the product <see cref="Miner.Interop.Process.IMMWMSDesign" /> interface into an workable object.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ID = {ID}")]
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public class Design : BasePxNode, IPxDesign
    {
        #region Constants

        /// <summary>
        ///     The name of the node type.
        /// </summary>
        internal const string NodeTypeName = "Design";

        #endregion

        #region Fields

        /// <summary>
        ///     The design object.
        /// </summary>
        private IMMWMSDesign _Design;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Design" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        public Design(IMMPxApplication pxApp)
            : base(pxApp, NodeTypeName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Design" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="design">The design.</param>
        public Design(IMMPxApplication pxApp, IMMWMSDesign design)
            : base(pxApp, NodeTypeName)
        {
            _Design = design;
        }

        #endregion

        #region IPxDesign Members

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Design" /> can post.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="Design" /> can post; otherwise, <c>false</c>.
        /// </value>
        public bool CanPost
        {
            get { return ((IMMWMSDesign4) _Design).CanPost; }
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
            get { return _Design.get_Description(); }
            set
            {
                if (value != null && value.Length > 32)
                {
                    throw new ArgumentOutOfRangeException("value", @"The description cannot be larger then 128 characters.");
                }

                _Design.set_Description(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Design" /> is view only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="Design" /> is view only; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewOnly
        {
            get { return ((IMMWMSDesign3) _Design).ViewOnly; }
        }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public IMMPxUser Owner
        {
            get { return base.PxApplication.GetUser(_Design.get_OwnerID()); }
            set
            {
                int ownerID = value.Id;
                _Design.set_OwnerID(ref ownerID);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the type of the product.
        /// </summary>
        /// <value>
        ///     The type of the product.
        /// </value>
        public mmWMSDesignerProductType ProductType
        {
            get { return ((IMMWMSDesign4) _Design).DesignerProductType; }
            set
            {
                ((IMMWMSDesign4) _Design).DesignerProductType = value;

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Design" /> is redlining.
        /// </summary>
        /// <value><c>true</c> if the <see cref="Design" /> is redlining; otherwise, <c>false</c>.</value>
        public bool Redlining
        {
            get { return ((IMMWMSDesign3) _Design).Redlining; }
            set
            {
                ((IMMWMSDesign3) _Design).Redlining = value;

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the work request ID.
        /// </summary>
        /// <value>
        ///     The work request ID.
        /// </value>
        public int WorkRequestID
        {
            get { return _Design.get_WorkRequestID(); }
            set { _Design.set_WorkRequestID(ref value); }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        public override int ID
        {
            get { return (_Design != null) ? _Design.ID : -1; }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Design" /> is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="Design" /> is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen
        {
            get
            {
                Design design = base.PxApplication.GetDesign();
                if (design == null) return false;

                bool isOpen = (design.ID == this.ID);
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
            get { return _Design.get_Name(); }
            set
            {
                if (value != null && value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("value", @"The name cannot be larger then 64 characters.");
                }

                _Design.set_Name(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Design" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public override bool Valid
        {
            get { return (_Design != null); }
        }

        /// <summary>
        /// Loads the package XML from the underlying workspace.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="string" /> representing the design XML; otherwise <c>null</c>
        /// </returns>
        /// <exception cref="NullReferenceException">The process framework workspace is null.</exception>
        public string GetDesignXml()
        {
            IWorkspace workspace = ((IMMPxApplicationEx2) base.PxApplication).Workspace;
            if(workspace == null)
                throw new NullReferenceException("The process framework workspace is null.");

            string name = this.ID.ToString(CultureInfo.InvariantCulture);
            IMMPackageName packageName = new MMPackageNameClass();
            packageName.Category = mmPackageCategory.mmPCDesignXML;
            packageName.Type = mmPackageType.mmPTHidden;
            packageName.Name = name;

            IMMPackageByWS packageByWs = new MMPackageManagerClass();
            IMMPackage package = packageByWs.GetPackageByWS(packageName, workspace, name);
            if (package != null)
            {
                IStream stream = new XMLStreamClass();
                package.Contents.SaveToStream(stream);
                string xml = ((IXMLStream) stream).SaveToString();
                return xml;
            }

            return null;
        }

        /// <summary>
        ///     Creates the process framework node wrapper for the specified the <paramref name="user" />.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully created; otherwise <c>false</c>.
        /// </returns>
        public override bool CreateNew(IMMPxUser user)
        {
            // Create the Design object.
            IMMWorkflowManager wm = base.PxApplication.GetWorkflowManager();
            if (wm == null) return false;

            int ownerID = user.Id;
            string nodeTypeName = NodeTypeName;

            _Design = (IMMWMSDesign) wm.CreateWMSNode(ref nodeTypeName);
            _Design.set_OwnerID(ref ownerID);

            this.Update();

            return (_Design != null);
        }

        /// <summary>
        ///     Deletes the <see cref="IMMPxNode" /> from the process framework database.
        /// </summary>
        public override void Delete()
        {
            // Delete the node.
            base.Delete();

            // Remove the design reference.
            this.Dispose(true);
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="nodeID" /> for the node.
        /// </summary>
        /// <param name="nodeID">The node ID.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        public override bool Initialize(int nodeID)
        {
            // Verify that the existing session isn't the same node.
            if (_Design != null && _Design.ID == nodeID)
                return true;

            // Reference the Design object.
            IMMWorkflowManager wm = base.PxApplication.GetWorkflowManager();
            if (wm == null) return false;

            bool ro = false;
            bool sm = true;
            string nodeTypeName = NodeTypeName;

            _Design = (IMMWMSDesign) wm.GetWMSNode(ref nodeTypeName, ref nodeID, ref ro, ref sm);

            return (_Design != null);
        }

        /// <summary>
        ///     Updates the entity by flushing the <see cref="IMMPxNode" /> to the database and reinitializing the
        ///     <see cref="IMMPxNode" />.
        /// </summary>
        public override void Update()
        {
            if (_Design != null)
            {
                ((IMMWMSNode) _Design).UpdateDB();

                base.Update();
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
                if (_Design != null)
                    while (Marshal.ReleaseComObject(_Design) > 0)
                    {
                    }

                _Design = null;
            }            

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Gets the list builder for the node.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="IMMListBuilder" /> representing the builder for the node.
        /// </returns>
        protected override IMMListBuilder GetListBuilder()
        {
            IMMPxListBuilderInit builder = new clsDesignBuilderClass();
            builder.PxApplication = base.PxApplication;

            return (IMMListBuilder) builder;
        }

        #endregion

        public IXMLDOMDocument ToXml()
        {
            IMMWorkflowManager wm = base.PxApplication.GetWorkflowManager();
            if (wm == null) return null;

            IMMMobileWFM mobileWfm = (IMMMobileWFM)wm;
            return mobileWfm.GetXML(this.Node);
        }
    }
}