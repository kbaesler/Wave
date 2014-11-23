using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Wraps the product <see cref="Miner.Interop.Process.IMMSession" /> interface into an workable object.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ID = {ID}, Owner = {Owner}")]
    public class Session : BasePxNode, IPxSession
    {
        #region Constants

        /// <summary>
        ///     The name of the node type.
        /// </summary>
        internal const string NodeTypeName = "Session";

        #endregion

        #region Fields

        /// <summary>
        ///     TMM Session object.
        /// </summary>
        private IMMSession _Session;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Session" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        public Session(IMMPxApplication pxApp)
            : base(pxApp, NodeTypeName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Session" /> class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="session">The session.</param>
        public Session(IMMPxApplication pxApp, IMMSession session)
            : base(pxApp, NodeTypeName)
        {
            _Session = session;
        }

        #endregion

        #region IPxSession Members

        /// <summary>
        ///     Gets or sets the create date.
        /// </summary>
        /// <value>The create date.</value>
        public DateTime CreateDate
        {
            get { return _Session.get_CreateDate(); }
            set
            {
                _Session.set_CreateDate(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the create user.
        /// </summary>
        /// <value>
        ///     The create user.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The create user cannot be larger then 32 characters.</exception>
        public string CreateUser
        {
            get { return _Session.get_CreateUser(); }
            set
            {
                if (value.Length > 32)
                {
                    throw new ArgumentOutOfRangeException("value", @"The create user cannot be larger then 32 characters.");
                }

                _Session.set_CreateUser(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the database identifier.
        /// </summary>
        /// <value>
        ///     The database.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The database cannot be larger then 64 characters.</exception>
        public string Database
        {
            get { return ((IMMSession3) _Session).get_DatabaseId(); }
            set
            {
                if (value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("value", @"The database cannot be larger then 64 characters.");
                }

                ((IMMSession3) _Session).set_DatabaseId(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The description cannot be larger then 255 characters.</exception>
        public string Description
        {
            get { return _Session.get_Description(); }
            set
            {
                if (value.Length > 255)
                {
                    throw new ArgumentOutOfRangeException("value", @"The description cannot be larger then 255 characters.");
                }

                _Session.set_Description(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the enterprise identifier.
        /// </summary>
        /// <value>
        ///     The enterprise.
        /// </value>
        public int Enterprise
        {
            get { return ((IMMSession3) _Session).get_EnterpriseId(); }
            set
            {
                ((IMMSession3) _Session).set_EnterpriseId(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxSession" /> is view only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IPxSession" /> is view only; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewOnly
        {
            get { return ((IMMSessionEx) _Session).get_ViewOnly(); }
        }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>
        ///     The owner.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The owner cannot be larger then 32 characters.</exception>
        public string Owner
        {
            get { return _Session.get_Owner(); }
            set
            {
                if (value.Length > 32)
                {
                    throw new ArgumentOutOfRangeException("value", @"The owner cannot be larger then 32 characters.");
                }

                _Session.set_Owner(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IPxSession" /> is redlining.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="IPxSession" /> is redlining; otherwise, <c>false</c>.
        /// </value>
        public bool Redlining
        {
            get { return ((IMMSession4) _Session).get_Redlining(); }
            set
            {
                ((IMMSession4) _Session).set_Redlining(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the type of the session.
        /// </summary>
        /// <value>
        ///     The type of the session.
        /// </value>
        public mmSessionNodeType SessionType
        {
            get { return (mmSessionNodeType) ((IMMSession3) _Session).get_SessionTypeId(); }
            set
            {
                int sessionType = (int) value;
                ((IMMSession3) _Session).set_SessionTypeId(ref sessionType);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        public override int ID
        {
            get { return (_Session != null) ? _Session.get_ID() : -1; }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxSession" /> is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IPxSession" /> is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen
        {
            get
            {
                Session session = base.PxApplication.GetSession();
                if (session == null) return false;

                bool isOpen = (session.ID == this.ID);
                return isOpen;
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The name cannot be larger then 64 characters.</exception>
        public override string Name
        {
            get { return _Session.get_Name(); }
            set
            {
                if (value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("value", @"The name cannot be larger then 64 characters.");
                }

                _Session.set_Name(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxSession" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public override bool Valid
        {
            get { return (_Session != null); }
        }

        /// <summary>
        ///     Creates the process framework node wrapper for the specified the <paramref name="user" />.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <returns>
        ///     Returns <see cref="Boolean" /> representing <c>true</c> if the node was successfully created; otherwise
        ///     <c>false</c>.
        /// </returns>
        public override bool CreateNew(IMMPxUser user)
        {
            // Create the product Session object.
            IMMSessionManager sm = base.PxApplication.GetSessionManager();
            if (sm == null) return false;

            string createUser = user.Name;

            _Session = sm.CreateSession();
            _Session.set_CreateUser(ref createUser);

            return (_Session != null);
        }

        /// <summary>
        ///     Deletes the <see cref="IMMPxNode" /> from the process framework database.
        /// </summary>
        public override void Delete()
        {
            // Delete the node.
            base.Delete();

            // Remove the session reference.
            _Session = null;
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="nodeID" /> for the node.
        /// </summary>
        /// <param name="nodeID">The node ID.</param>
        /// <returns>
        ///     Returns <see cref="Boolean" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        public override bool Initialize(int nodeID)
        {
            // Verify that the existing session isn't the same node.
            if (_Session != null && _Session.get_ID() == nodeID)
                return true;

            // Reference the TM&M Session object.
            IMMSessionManager sm = base.PxApplication.GetSessionManager();
            if (sm == null) return false;

            bool ro = false;

            _Session = sm.GetSession(ref nodeID, ref ro);

            return (_Session != null);
        }

        /// <summary>
        ///     Updates the entity by flushing the <see cref="IMMPxNode" /> to the database and reinitializing the
        ///     <see cref="IMMPxNode" />.
        /// </summary>
        public override void Update()
        {
            // Clear the cache by calling get session with -1.
            int sessionID = -1;
            bool ro = false;
            IMMSessionManager sm = base.PxApplication.GetSessionManager();
            sm.GetSession(ref sessionID, ref ro);

            // Call the base implementation.
            base.Update();
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
                if (_Session != null)
                    while (Marshal.ReleaseComObject(_Session) > 0)
                    {
                    }
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
            return null;
        }

        #endregion
    }
}