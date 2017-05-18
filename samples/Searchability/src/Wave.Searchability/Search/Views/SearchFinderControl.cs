using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;

using Wave.Searchability.Data;
using Wave.Searchability.Services;

namespace Wave.Searchability.Search.Views
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    public partial class SearchFinderControl : UserControl
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchFinderControl" /> class.
        /// </summary>
        public SearchFinderControl()
        {
            InitializeComponent();

            EventAggregator.GetEvent<SearchableItemsEvent>().Subscribe(items =>
            {
                foreach (var s in items)
                {
                    EventAggregator.GetEvent<SearchableItemEvent>().Publish(s);
                }
            });

            EventAggregator.GetEvent<SearchableItemEvent>().Subscribe(s =>
            {
                var package = s as SearchablePackage;
                if (package != null)
                {
                    if (package.Items.Any())
                    {
                        var node = this.tvwPackages.Nodes.Add(s.AliasName);
                        node.ImageIndex = 0;
                        node.Tag = package;

                        foreach (var i in package.Items)
                        {
                            var child = node.Nodes.Add(i.AliasName);
                            child.Tag = i;
                            child.ImageIndex = (int) i.ItemType;
                        }
                    }
                }
                else
                {
                    var item = s as SearchableItem;
                    if (item != null)
                    {
                        var node = this.tvwPackages.Nodes.Add(s.AliasName);
                        node.ImageIndex = (int) item.ItemType;
                        node.Tag = item;
                    }
                }
            });

            var comparisonOperators = new Dictionary<string, ComparisonOperator>
            {
                {"Contains", ComparisonOperator.Contains},
                {"Start With", ComparisonOperator.StartsWith},
                {"Ends With", ComparisonOperator.EndsWith},
                {"Equals", ComparisonOperator.Equals}
            };

            this.cboComparisonOperator.DataSource = new BindingSource(comparisonOperators, null);
            this.cboComparisonOperator.DisplayMember = "Key";
            this.cboComparisonOperator.ValueMember = "Value";

            var extents = new Dictionary<string, MapSearchServiceExtent>
            {
                {"Any Extent", MapSearchServiceExtent.WithinAnyExtent},
                {"Current Extent", MapSearchServiceExtent.WithinCurrentExtent},
                {"Overlapping Current Extent", MapSearchServiceExtent.WithinOrOverlappingCurrentExtent}
            };

            this.cboExtent.DataSource = new BindingSource(extents, null);
            this.cboExtent.DisplayMember = "Key";
            this.cboExtent.ValueMember = "Value";

            this.cboPackages.PopupContainer.Popup += PopupContainer_OnPopup;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.tvwPackages.Nodes.Clear();
            this.cboFind.Items.Clear();
            this.btnAllFields.PerformClick();
        }

        /// <summary>
        ///     Gets the service request.
        /// </summary>
        /// <returns>Returns a <see cref="MapSearchServiceRequest" /> representing a request for the searches.</returns>
        public MapSearchServiceRequest GetSearchRequest()
        {
            var package = this.GetSearchablePackage();

            var request = new MapSearchServiceRequest
            {
                ComparisonOperator = ((KeyValuePair<string, ComparisonOperator>) this.cboComparisonOperator.SelectedItem).Value,
                Extent = ((KeyValuePair<string, MapSearchServiceExtent>) this.cboExtent.SelectedItem).Value,
                Keyword = this.cboFind.Text,
                Threshold = (int) this.txtThreshold.Value,
                ThresholdConstraint = ThresholdConstraints.Item,
                Packages = new List<SearchablePackage>(new[] {package})
            };

            if (!this.cboFind.Items.Contains(this.cboFind.Text))
                this.cboFind.Items.Add(this.cboFind.Text);

            return request;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Finds the node.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private TreeNode FindNode(TreeNodeCollection nodes, string text)
        {
            foreach (TreeNode child in nodes)
                if (child.Text == text)
                    return child;

            foreach (TreeNode child in nodes)
            {
                TreeNode matched = this.FindNode(child.Nodes, text);
                if (matched != null)
                    return matched;
            }

            return null;
        }

        /// <summary>
        ///     Gets the checked fields.
        /// </summary>
        /// <returns>Returns a <see cref="IEnumerable{T}" /> representing the checked fields.</returns>
        private SearchableField[] GetCheckedFields()
        {
            var nodes = this.cboFields.SelectedItems.OfType<SearchableField>();
            return nodes.ToArray();
        }


        /// <summary>
        ///     Gets (copies) of the searchable items.
        /// </summary>
        /// <returns>Returns a <see cref="IEnumerable{T}" /> representing the items that will be searched.</returns>
        private SearchableItem[] GetSearchableItems()
        {
            IEnumerable<SearchableItem> items;
            var node = this.tvwPackages.SelectedNode;
            var package = node.Tag as SearchablePackage;
            if (package != null)
            {
                items = package.Items;
            }
            else
            {
                var item = node.Tag as SearchableItem;
                items = new[] {item};
            }

            return items.Select(Searchable.Copy).ToArray();
        }

        /// <summary>
        ///     Gets the searchable package.
        /// </summary>
        /// <returns>Returns a <see cref="SearchablePackage" /> representing the package.</returns>
        private SearchablePackage GetSearchablePackage()
        {
            var items = this.GetSearchableItems();
            var fields = this.GetCheckedFields();
            if (fields.Any())
            {
                foreach (var item in items)
                {
                    var contains = item.Fields.Where(fields.Contains).ToList();
                    if (contains.Any())
                    {
                        item.Fields.Remove(f => !contains.Contains(f));
                    }
                }
            }

            var package = new SearchablePackage(this.tvwPackages.SelectedNode.Text, items);
            return package;
        }

        /// <summary>
        ///     Populates the fields drop down.
        /// </summary>
        /// <param name="items">The items.</param>
        private void PopulateFieldsDropDown(params SearchableItem[] items)
        {
            this.cboFields.Items.Clear();

            var fields = new List<SearchableField>();

            foreach (var item in items)
            {
                foreach (var field in item.Fields.Where(f => !f.Equals(SearchableField.Any)))
                {
                    if (!fields.Contains(field))
                    {
                        fields.Add(field);
                    }
                }
            }

            this.cboFields.DataSource = new BindingSource(fields, null);
            this.cboFields.DisplayMember = "Key";
            this.cboFields.ValueMember = "Value";
        }

        /// <summary>
        ///     Populates the fields drop down.
        /// </summary>
        private void PopulateFieldsDropDown()
        {
            var node = this.tvwPackages.SelectedNode;
            if (node != null)
            {
                var package = node.Tag as SearchablePackage;
                if (package != null)
                {
                    this.PopulateFieldsDropDown(package.Items.ToArray());
                }
                else
                {
                    var item = node.Tag as SearchableItem;

                    this.PopulateFieldsDropDown(item);
                }
            }
        }

        /// <summary>
        ///     Handles the OnPopup event of the PopupContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void PopupContainer_OnPopup(object sender, EventArgs eventArgs)
        {
            this.tvwPackages.Focus();
        }

        /// <summary>
        ///     Handles the CheckedChanged event of the btnAllFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void btnAllFields_CheckedChanged(object sender, EventArgs e)
        {
            this.btnFields.Checked = ! (((RadioButton) sender).Checked);

            this.PopulateFieldsDropDown();
        }

        /// <summary>
        ///     Handles the CheckedChanged event of the btnFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void btnFields_CheckedChanged(object sender, EventArgs e)
        {
            this.cboFields.Enabled = ((RadioButton) sender).Checked;
        }
       
        /// <summary>
        ///     Handles the DropDown event of the cboPackages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cboPackages_DropDown(object sender, EventArgs e)
        {
            this.cboPackages.PopupContainer.ParentControl = this;
        }

        /// <summary>
        ///     Handles the SelectionChangeCommitted event of the cboPackages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cboPackages_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.PopulateFieldsDropDown();
        }

        /// <summary>
        ///     Handles the AfterSelect event of the tvwPackages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TreeViewEventArgs" /> instance containing the event data.</param>
        private void tvwPackages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvwPackages.SelectedNode != null)
                this.cboPackages.Text = this.tvwPackages.SelectedNode.Text;
            else
                this.cboPackages.Text = string.Empty;

            this.cboPackages.PopupContainer.HidePopup(PopupCloseType.Done);
        }

        #endregion
    }
}