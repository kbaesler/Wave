using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoApp {

	public partial class DemoForm : Form {

		public DemoForm() {
			InitializeComponent();

			rbPlain.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
			rbVS.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);

			// define our collection of list items
			var groupedItems = new[] { 
				new { Group = "Gases", Value = 1, Display = "Helium" }, 
				new { Group = "Gases", Value = 2, Display = "Hydrogen" },
				new { Group = "Gases", Value = 3, Display = "Oxygen" },
				new { Group = "Gases", Value = 4, Display = "Argon" },
				new { Group = "Metals", Value = 5, Display = "Iron" },
				new { Group = "Metals", Value = 6, Display = "Lithium" },
				new { Group = "Metals", Value = 7, Display = "Copper" },
				new { Group = "Metals", Value = 8, Display = "Gold" },
				new { Group = "Metals", Value = 9, Display = "Silver" },
				new { Group = "Radioactive", Value = 10, Display = "Uranium" },
				new { Group = "Radioactive", Value = 11, Display = "Plutonium" },
				new { Group = "Radioactive", Value = 12, Display = "Americium" },
				new { Group = "Radioactive", Value = 13, Display = "Radon" }
			};

			Action<ComboTreeNodeCollection> addNodesHelper = nodes => {
				foreach (var grp in groupedItems.GroupBy(x => x.Group)) {
					ComboTreeNode parent = nodes.Add(grp.Key);
					foreach (var item in grp) {
						parent.Nodes.Add(item.Display);
					}
				}
			};

			// anonymous method delegate for transforming the above into nodes
			Action<ComboTreeBox> addNodes = ctb => {
				addNodesHelper(ctb.Nodes);
				ctb.Sort();
				ctb.SelectedNode = ctb.Nodes[0].Nodes[0];
			};

			// normal combobox
			cmbNormal.ValueMember = "Value";
			cmbNormal.DisplayMember = "Display";
			cmbNormal.DataSource = new BindingSource(groupedItems, String.Empty);

			// grouped comboboxes
			gcbList.ValueMember = "Value";
			gcbList.DisplayMember = "Display";
			gcbList.GroupMember = "Group";
			gcbList.DataSource = new BindingSource(groupedItems, String.Empty);

			gcbEditable.ValueMember = "Value";
			gcbEditable.DisplayMember = "Display";
			gcbEditable.GroupMember = "Group";
			gcbEditable.DataSource = new BindingSource(groupedItems, String.Empty);

			// combotreeboxes
			addNodes(ctbNormal);
			addNodes(ctbImages);
			
			addNodes(ctbCheckboxes);
			ctbCheckboxes.CheckedNodes = new ComboTreeNode[] { 
				ctbCheckboxes.Nodes[1].Nodes[0], 
				ctbCheckboxes.Nodes[1].Nodes[1] 
			};

			foreach (var item in groupedItems) {
				ctbFlatChecks.Nodes.Add(item.Display);
			}

			ctbFlatChecks.CheckedNodes = new ComboTreeNode[] { 
				ctbFlatChecks.Nodes[0], 
				ctbFlatChecks.Nodes[1] 
			};

			// datagridview columns
			Column1.ValueMember = "Value";
			Column1.DisplayMember = "Display";
			Column1.GroupMember = "Group";
			Column1.DataSource = new BindingSource(groupedItems, String.Empty);

			Column2.Images = imageList;
			Column2.ImageIndex = 0;
			Column2.ExpandedImageIndex = 1;
			addNodesHelper(Column2.Nodes);
		}

		void radioButtons_CheckedChanged(object sender, EventArgs e) {
			ctbNormal.DrawWithVisualStyles = rbVS.Checked;
			ctbImages.DrawWithVisualStyles = rbVS.Checked;
			ctbCheckboxes.DrawWithVisualStyles = rbVS.Checked;
			ctbFlatChecks.DrawWithVisualStyles = rbVS.Checked;
		}
	}
}
