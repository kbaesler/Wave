namespace Wave.Searchability.Search.Views
{
    partial class SearchFinderControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchFinderControl));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.stateImageList = new System.Windows.Forms.ImageList(this.components);
            this.cboExtent = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboComparisonOperator = new System.Windows.Forms.ComboBox();
            this.btnFields = new System.Windows.Forms.RadioButton();
            this.btnAllFields = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.cboFind = new System.Windows.Forms.ComboBox();
            this.txtThreshold = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.cboPackages = new Syncfusion.Windows.Forms.Tools.ComboDropDown();
            this.tvwPackages = new System.Windows.Forms.TreeView();
            this.cboFields = new Syncfusion.Windows.Forms.Tools.MultiSelectionComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPackages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboFields)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "In:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Find:";
            // 
            // stateImageList
            // 
            this.stateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("stateImageList.ImageStream")));
            this.stateImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.stateImageList.Images.SetKeyName(0, "LayerGroup16.png");
            this.stateImageList.Images.SetKeyName(1, "LayerAnnotation16.png");
            this.stateImageList.Images.SetKeyName(2, "LayerDimension16.png");
            this.stateImageList.Images.SetKeyName(3, "LayerLine16.png");
            this.stateImageList.Images.SetKeyName(4, "LayerPoint16.png");
            this.stateImageList.Images.SetKeyName(5, "LayerPolygon16.png");
            this.stateImageList.Images.SetKeyName(6, "TableStandaloneLayer16.png");
            // 
            // cboExtent
            // 
            this.cboExtent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboExtent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExtent.FormattingEnabled = true;
            this.cboExtent.Location = new System.Drawing.Point(216, 69);
            this.cboExtent.Name = "cboExtent";
            this.cboExtent.Size = new System.Drawing.Size(453, 24);
            this.cboExtent.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(158, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Within:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "When:";
            // 
            // cboComparisonOperator
            // 
            this.cboComparisonOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboComparisonOperator.FormattingEnabled = true;
            this.cboComparisonOperator.Location = new System.Drawing.Point(60, 69);
            this.cboComparisonOperator.Name = "cboComparisonOperator";
            this.cboComparisonOperator.Size = new System.Drawing.Size(92, 24);
            this.cboComparisonOperator.TabIndex = 9;
            // 
            // btnFields
            // 
            this.btnFields.AutoSize = true;
            this.btnFields.Location = new System.Drawing.Point(21, 177);
            this.btnFields.Name = "btnFields";
            this.btnFields.Size = new System.Drawing.Size(82, 21);
            this.btnFields.TabIndex = 23;
            this.btnFields.Text = "In Fields:";
            this.btnFields.UseVisualStyleBackColor = true;
            this.btnFields.CheckedChanged += new System.EventHandler(this.btnFields_CheckedChanged);
            // 
            // btnAllFields
            // 
            this.btnAllFields.AutoSize = true;
            this.btnAllFields.Location = new System.Drawing.Point(21, 150);
            this.btnAllFields.Name = "btnAllFields";
            this.btnAllFields.Size = new System.Drawing.Size(77, 21);
            this.btnAllFields.TabIndex = 22;
            this.btnAllFields.Text = "All Fields";
            this.btnAllFields.UseVisualStyleBackColor = true;
            this.btnAllFields.CheckedChanged += new System.EventHandler(this.btnAllFields_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 17);
            this.label5.TabIndex = 21;
            this.label5.Text = "Search:";
            // 
            // cboFind
            // 
            this.cboFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFind.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboFind.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboFind.FormattingEnabled = true;
            this.cboFind.Location = new System.Drawing.Point(60, 10);
            this.cboFind.Name = "cboFind";
            this.cboFind.Size = new System.Drawing.Size(608, 24);
            this.cboFind.TabIndex = 25;
            // 
            // txtThreshold
            // 
            this.txtThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThreshold.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtThreshold.Location = new System.Drawing.Point(612, 99);
            this.txtThreshold.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txtThreshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(56, 23);
            this.txtThreshold.TabIndex = 26;
            this.txtThreshold.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(533, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 17);
            this.label6.TabIndex = 27;
            this.label6.Text = "Threshold:";
            // 
            // cboPackages
            // 
            this.cboPackages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPackages.BeforeTouchSize = new System.Drawing.Size(608, 24);
            this.cboPackages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPackages.IgnoreThemeBackground = false;
            this.cboPackages.Location = new System.Drawing.Point(60, 40);
            this.cboPackages.Name = "cboPackages";
            this.cboPackages.PopupControl = this.tvwPackages;
            this.cboPackages.Size = new System.Drawing.Size(608, 24);
            this.cboPackages.TabIndex = 28;
            this.cboPackages.DropDown += new System.EventHandler(this.cboPackages_DropDown);
            // 
            // tvwPackages
            // 
            this.tvwPackages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwPackages.ImageIndex = 0;
            this.tvwPackages.ImageList = this.stateImageList;
            this.tvwPackages.Location = new System.Drawing.Point(60, 40);
            this.tvwPackages.Name = "tvwPackages";
            this.tvwPackages.SelectedImageIndex = 0;
            this.tvwPackages.Size = new System.Drawing.Size(608, 24);
            this.tvwPackages.TabIndex = 1;
            this.tvwPackages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwPackages_AfterSelect);
            // 
            // cboFields
            // 
            this.cboFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFields.BeforeTouchSize = new System.Drawing.Size(608, 24);
            this.cboFields.ButtonStyle = Syncfusion.Windows.Forms.ButtonAppearance.Classic;
            this.cboFields.DataSource = ((object)(resources.GetObject("cboFields.DataSource")));
            this.cboFields.DisplayMode = Syncfusion.Windows.Forms.Tools.DisplayMode.NormalMode;
            this.cboFields.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboFields.Location = new System.Drawing.Point(60, 204);
            this.cboFields.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.cboFields.Name = "cboFields";
            this.cboFields.ShowCheckBox = true;
            this.cboFields.Size = new System.Drawing.Size(608, 24);
            this.cboFields.TabIndex = 29;
            this.cboFields.UseVisualStyle = true;
            this.cboFields.VisualItemBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // SearchFinderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboFields);
            this.Controls.Add(this.cboPackages);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtThreshold);
            this.Controls.Add(this.cboFind);
            this.Controls.Add(this.btnFields);
            this.Controls.Add(this.btnAllFields);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboComparisonOperator);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboExtent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tvwPackages);
            this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SearchFinderControl";
            this.Size = new System.Drawing.Size(677, 237);
            ((System.ComponentModel.ISupportInitialize)(this.txtThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPackages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboFields)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ImageList stateImageList;
        private System.Windows.Forms.ComboBox cboExtent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboComparisonOperator;
        private System.Windows.Forms.RadioButton btnFields;
        private System.Windows.Forms.RadioButton btnAllFields;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboFind;
        private System.Windows.Forms.NumericUpDown txtThreshold;
        private System.Windows.Forms.Label label6;
        private Syncfusion.Windows.Forms.Tools.ComboDropDown cboPackages;
        private System.Windows.Forms.TreeView tvwPackages;
        private Syncfusion.Windows.Forms.Tools.MultiSelectionComboBox cboFields;
    }
}
