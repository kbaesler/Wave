using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Forms.Controls
{
    /// <summary>
    ///     Represents a node in the ComboTreeBox. A node may have a name, text, font style, image and
    ///     may contain child nodes. If so, it can be expanded or collapsed.
    /// </summary>
    /// <remarks>http://www.brad-smith.info/blog/archives/193</remarks>
    [DefaultProperty("Text")]
    public class ComboTreeNode : IComparable<ComboTreeNode>
    {
        #region Fields

        private readonly ComboTreeNodeCollection _Nodes;

        private string _Name;
        private ComboTreeNode _Parent;
        private string _Text;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialises a new instance of ComboTreeNode using default (empty) values.
        /// </summary>
        public ComboTreeNode()
        {
            _Nodes = new ComboTreeNodeCollection(this);
            _Name = _Text = String.Empty;
            FontStyle = FontStyle.Regular;
            ExpandedImageIndex = ImageIndex = -1;
            ExpandedImageKey = ImageKey = String.Empty;
            Expanded = false;
        }

        /// <summary>
        ///     Initialises a new instance of ComboTreeNode with the specified text.
        /// </summary>
        /// <param name="text"></param>
        public ComboTreeNode(string text)
            : this()
        {
            _Text = text;
        }

        /// <summary>
        ///     Initialises a new instance of ComboTreeNode with the specified name and text.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public ComboTreeNode(string name, string text)
            : this()
        {
            _Text = text;
            _Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Determines the zero-based depth of the node, relative to the ComboTreeBox control.
        /// </summary>
        [Browsable(false)]
        public int Depth
        {
            get
            {
                int depth = 0;
                ComboTreeNode node = this;
                while ((node = node._Parent) != null) depth++;
                return depth;
            }
        }

        /// <summary>
        ///     Gets or sets whether the node is expanded (i.e. its child nodes are visible). Changes are not reflected in the
        ///     dropdown portion of the
        ///     control until the next time it is opened.
        /// </summary>
        [Browsable(false)]
        public bool Expanded { get; set; }

        /// <summary>
        ///     Gets or sets the index of the image to use for this node when expanded.
        /// </summary>
        [DefaultValue(-1),
         Description("The index of the image to use for this node when expanded."),
         Category("Appearance")]
        public int ExpandedImageIndex { get; set; }

        /// <summary>
        ///     Gets or sets the name of the image to use for this node when expanded.
        /// </summary>
        [DefaultValue(""),
         Description("The name of the image to use for this node when expanded."),
         Category("Appearance")]
        public string ExpandedImageKey { get; set; }

        /// <summary>
        ///     Gets or sets the font style to use when painting the node.
        /// </summary>
        [DefaultValue(FontStyle.Regular),
         Description("The font style to use when painting the node."),
         Category("Appearance")]
        public FontStyle FontStyle { get; set; }

        /// <summary>
        ///     Gets or sets the index of the image (in the ImageList on the ComboTreeBox control) to use for this node.
        /// </summary>
        [DefaultValue(-1),
         Description("The index of the image (in the ImageList on the ComboTreeBox control) to use for this node."),
         Category("Appearance")]
        public int ImageIndex { get; set; }

        /// <summary>
        ///     Gets or sets the name of the image to use for this node.
        /// </summary>
        [DefaultValue(""),
         Description("The name of the image to use for this node."),
         Category("Appearance")]
        public string ImageKey { get; set; }

        /// <summary>
        ///     Gets or sets the name of the node.
        /// </summary>
        [Description("The name of the node."),
         DefaultValue(""),
         Category("Design")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        ///     Gets a collection of the child nodes for this node.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("The collection of the child nodes for this node."),
         Category("Data")]
        public ComboTreeNodeCollection Nodes
        {
            get { return _Nodes; }
        }

        /// <summary>
        ///     Gets or sets the node that owns this node, or null for a top-level node.
        /// </summary>
        [Browsable(false)]
        public ComboTreeNode Parent
        {
            get { return _Parent; }
            internal set { _Parent = value; }
        }

        /// <summary>
        ///     Gets or sets a user-defined object associated with this ComboTreeNode.
        /// </summary>
        [Description("User-defined object associated with this ComboTreeNode."),
         DefaultValue(null),
         Category("Data")]
        public object Tag { get; set; }

        /// <summary>
        ///     Gets or sets the text displayed on the node.
        /// </summary>
        [DefaultValue("ComboTreeNode"),
         Description("The text displayed on the node."),
         Category("Appearance")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        #endregion

        #region IComparable<ComboTreeNode> Members

        /// <summary>
        ///     Compares two ComboTreeNode objects using a culture-invariant, case-insensitive comparison of the Text property.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ComboTreeNode other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(_Text, other._Text);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as ComboTreeNode;
            if (other == null) return false;

            return string.Equals(Text, other.Text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new {A = Depth, B = Text, C = Tag, D = Parent, E = Name}.GetHashCode();
        }

        /// <summary>
        ///     Returns a string representation of this ComboTreeNode.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Name=\"{0}\", Text=\"{1}\"", _Name, _Text);
        }

        #endregion
    }
}