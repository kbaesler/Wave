using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An interface used to construct a process framework design entity.
    /// </summary>
    [ComVisible(false)]
    public interface IPxDesign : IPxNode
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxDesign" /> can post.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IPxDesign" /> can post; otherwise, <c>false</c>.
        /// </value>
        bool CanPost { get; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The description cannot be larger then 128 characters.</exception>
        string Description { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxDesign" /> is view only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IPxDesign" /> is view only; otherwise, <c>false</c>.
        /// </value>
        bool IsViewOnly { get; }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>
        ///     The owner.
        /// </value>
        IMMPxUser Owner { get; set; }

        /// <summary>
        ///     Gets or sets the type of the product.
        /// </summary>
        /// <value>
        ///     The type of the product.
        /// </value>
        mmWMSDesignerProductType ProductType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IPxDesign" /> is redlining.
        /// </summary>
        /// <value><c>true</c> if the <see cref="IPxDesign" /> is redlining; otherwise, <c>false</c>.</value>
        bool Redlining { get; set; }

        /// <summary>
        ///     Gets or sets the work request ID.
        /// </summary>
        /// <value>
        ///     The work request ID.
        /// </value>
        int WorkRequestID { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Loads the package XML given the <paramref name="workspace" />
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns>
        ///     Returns a <see cref="String" /> representing the design XML; otherwise <c>null</c>
        /// </returns>
        string GetDesignXml(IWorkspace workspace);

        #endregion
    }
}