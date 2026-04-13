namespace AikDemo
{
    partial class RequestFrm
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
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestFrm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnAddAttribute = new System.Windows.Forms.Button();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblDesignatorIdentifier = new System.Windows.Forms.Label();
            this.lblDesignatorDataType = new System.Windows.Forms.Label();
            this.lblDesignatorCategory = new System.Windows.Forms.Label();
            this.cmbIdentifier = new System.Windows.Forms.ComboBox();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.dgvAttributes = new System.Windows.Forms.DataGridView();
            this.clmnCat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRequestText = new System.Windows.Forms.Button();
            this.btnEvaluate = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnRequestText);
            this.splitContainer1.Panel2.Controls.Add(this.btnEvaluate);
            this.splitContainer1.Size = new System.Drawing.Size(619, 738);
            this.splitContainer1.SplitterDistance = 684;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnAddAttribute);
            this.splitContainer2.Panel1.Controls.Add(this.txtValue);
            this.splitContainer2.Panel1.Controls.Add(this.lblValue);
            this.splitContainer2.Panel1.Controls.Add(this.lblMessage);
            this.splitContainer2.Panel1.Controls.Add(this.lblDesignatorIdentifier);
            this.splitContainer2.Panel1.Controls.Add(this.lblDesignatorDataType);
            this.splitContainer2.Panel1.Controls.Add(this.lblDesignatorCategory);
            this.splitContainer2.Panel1.Controls.Add(this.cmbIdentifier);
            this.splitContainer2.Panel1.Controls.Add(this.cmbDataType);
            this.splitContainer2.Panel1.Controls.Add(this.cmbCategory);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgvAttributes);
            this.splitContainer2.Size = new System.Drawing.Size(619, 684);
            this.splitContainer2.SplitterDistance = 320;
            this.splitContainer2.TabIndex = 1;
            // 
            // btnAddAttribute
            // 
            this.btnAddAttribute.Image = ((System.Drawing.Image)(resources.GetObject("btnAddAttribute.Image")));
            this.btnAddAttribute.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAddAttribute.Location = new System.Drawing.Point(229, 260);
            this.btnAddAttribute.Name = "btnAddAttribute";
            this.btnAddAttribute.Size = new System.Drawing.Size(187, 37);
            this.btnAddAttribute.TabIndex = 40;
            this.btnAddAttribute.Text = "Add the Attribute to the Request";
            this.btnAddAttribute.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddAttribute.UseVisualStyleBackColor = true;
            this.btnAddAttribute.Click += new System.EventHandler(this.btnAddAttribute_Click);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(117, 222);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(433, 20);
            this.txtValue.TabIndex = 39;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(54, 225);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(34, 13);
            this.lblValue.TabIndex = 38;
            this.lblValue.Text = "Value";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(33, 24);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(245, 13);
            this.lblMessage.TabIndex = 37;
            this.lblMessage.Text = "Add XACML attributes to the authorization request:";
            // 
            // lblDesignatorIdentifier
            // 
            this.lblDesignatorIdentifier.AutoSize = true;
            this.lblDesignatorIdentifier.Location = new System.Drawing.Point(54, 114);
            this.lblDesignatorIdentifier.Name = "lblDesignatorIdentifier";
            this.lblDesignatorIdentifier.Size = new System.Drawing.Size(47, 13);
            this.lblDesignatorIdentifier.TabIndex = 33;
            this.lblDesignatorIdentifier.Text = "Identifier";
            // 
            // lblDesignatorDataType
            // 
            this.lblDesignatorDataType.AutoSize = true;
            this.lblDesignatorDataType.Location = new System.Drawing.Point(54, 168);
            this.lblDesignatorDataType.Name = "lblDesignatorDataType";
            this.lblDesignatorDataType.Size = new System.Drawing.Size(57, 13);
            this.lblDesignatorDataType.TabIndex = 32;
            this.lblDesignatorDataType.Text = "Data Type";
            // 
            // lblDesignatorCategory
            // 
            this.lblDesignatorCategory.AutoSize = true;
            this.lblDesignatorCategory.Location = new System.Drawing.Point(54, 64);
            this.lblDesignatorCategory.Name = "lblDesignatorCategory";
            this.lblDesignatorCategory.Size = new System.Drawing.Size(49, 13);
            this.lblDesignatorCategory.TabIndex = 31;
            this.lblDesignatorCategory.Text = "Category";
            // 
            // cmbIdentifier
            // 
            this.cmbIdentifier.FormattingEnabled = true;
            this.cmbIdentifier.Location = new System.Drawing.Point(117, 114);
            this.cmbIdentifier.Name = "cmbIdentifier";
            this.cmbIdentifier.Size = new System.Drawing.Size(433, 21);
            this.cmbIdentifier.TabIndex = 29;
            // 
            // cmbDataType
            // 
            this.cmbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Location = new System.Drawing.Point(117, 165);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(433, 21);
            this.cmbDataType.TabIndex = 30;
            // 
            // cmbCategory
            // 
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(117, 61);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(433, 21);
            this.cmbCategory.TabIndex = 28;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbDesignatorCategory_SelectedIndexChanged);
            // 
            // dgvAttributes
            // 
            this.dgvAttributes.AllowUserToAddRows = false;
            this.dgvAttributes.AllowUserToDeleteRows = false;
            this.dgvAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnCat,
            this.clmnId,
            this.clmnDataType,
            this.clmnValue});
            this.dgvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttributes.Location = new System.Drawing.Point(0, 0);
            this.dgvAttributes.Name = "dgvAttributes";
            this.dgvAttributes.ReadOnly = true;
            this.dgvAttributes.RowHeadersVisible = false;
            this.dgvAttributes.Size = new System.Drawing.Size(619, 360);
            this.dgvAttributes.TabIndex = 0;
            // 
            // clmnCat
            // 
            this.clmnCat.HeaderText = "Category";
            this.clmnCat.Name = "clmnCat";
            this.clmnCat.ReadOnly = true;
            // 
            // clmnId
            // 
            this.clmnId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnId.HeaderText = "Identifier";
            this.clmnId.Name = "clmnId";
            this.clmnId.ReadOnly = true;
            // 
            // clmnDataType
            // 
            this.clmnDataType.HeaderText = "DataType";
            this.clmnDataType.Name = "clmnDataType";
            this.clmnDataType.ReadOnly = true;
            // 
            // clmnValue
            // 
            this.clmnValue.HeaderText = "Value";
            this.clmnValue.Name = "clmnValue";
            this.clmnValue.ReadOnly = true;
            // 
            // btnRequestText
            // 
            this.btnRequestText.Location = new System.Drawing.Point(363, 15);
            this.btnRequestText.Name = "btnRequestText";
            this.btnRequestText.Size = new System.Drawing.Size(119, 23);
            this.btnRequestText.TabIndex = 1;
            this.btnRequestText.Text = "View Request Text";
            this.btnRequestText.UseVisualStyleBackColor = true;
            this.btnRequestText.Click += new System.EventHandler(this.btnRequestText_Click);
            // 
            // btnEvaluate
            // 
            this.btnEvaluate.Location = new System.Drawing.Point(488, 15);
            this.btnEvaluate.Name = "btnEvaluate";
            this.btnEvaluate.Size = new System.Drawing.Size(119, 23);
            this.btnEvaluate.TabIndex = 0;
            this.btnEvaluate.Text = "Evaluate Request";
            this.btnEvaluate.UseVisualStyleBackColor = true;
            this.btnEvaluate.Click += new System.EventHandler(this.btnEvaluate_Click);
            // 
            // RequestFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 738);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.Name = "RequestFrm";
            this.Text = "Request";
            this.Load += new System.EventHandler(this.RequestFrm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnAddAttribute;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblDesignatorIdentifier;
        private System.Windows.Forms.Label lblDesignatorDataType;
        private System.Windows.Forms.Label lblDesignatorCategory;
        private System.Windows.Forms.ComboBox cmbIdentifier;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.DataGridView dgvAttributes;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnCat;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnValue;
        private System.Windows.Forms.Button btnRequestText;
        private System.Windows.Forms.Button btnEvaluate;

    }
}