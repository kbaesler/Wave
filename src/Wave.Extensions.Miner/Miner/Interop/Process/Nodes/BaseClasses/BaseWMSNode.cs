using System;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract implemenation of the process framework nodes that don't correspond to a
    ///     <see cref="Miner.Interop.Process.IMMPxNode" />
    /// </summary>
    public abstract class BaseWMSNode : IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseWMSNode" /> class.
        /// </summary>
        /// <param name="node">The node.</param>
        protected BaseWMSNode(IMMWMSNode node)
        {
            this.Node = node;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BaseWMSNode" /> is valid.
        /// </summary>
        /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
        public bool Valid
        {
            get
            {
                if (this.Node == null)
                    return false;

                return this.Node.Valid;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="BaseWMSNode" /> is dirty.
        /// </summary>
        /// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
        protected bool Dirty
        {
            get
            {
                if (this.Node == null)
                    return false;

                return this.Node.Dirty;
            }
            set
            {
                if (this.Node == null)
                    return;

                this.Node.Dirty = value;
            }
        }

        /// <summary>
        ///     Gets the node.
        /// </summary>
        /// <value>The node.</value>
        protected IMMWMSNode Node { get; private set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Updates the node by flushing the information to the database.
        /// </summary>
        public virtual void Update()
        {
            if (this.Dirty)
            {
                if (this.Node != null)
                {
                    this.Node.UpdateDB();
                }
            }

            this.Dirty = false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Node != null)
                    while (Marshal.ReleaseComObject(this.Node) > 0)
                    {
                    }

                this.Node = null;
            }            
        }

        #endregion
    }
}