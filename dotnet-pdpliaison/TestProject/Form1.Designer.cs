namespace TestProject
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtResults = new System.Windows.Forms.TextBox();
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.chkReturnPolicy = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.requestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendSingleRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendMultipleRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.requestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.requestsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.requestsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.requestsToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.requestsToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.openRequestGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkVerify = new System.Windows.Forms.CheckBox();
            this.cmbComType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPdpUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCerts = new System.Windows.Forms.ComboBox();
            this.txtDN = new System.Windows.Forms.TextBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.txtConvDn = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbConnectorType = new System.Windows.Forms.ComboBox();
            this.chkCertChain = new System.Windows.Forms.CheckBox();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblExtension = new System.Windows.Forms.Label();
            this.txtXacmlExtension = new System.Windows.Forms.TextBox();
            this.btnAddExtension = new System.Windows.Forms.Button();
            this.lblNumberOf = new System.Windows.Forms.Label();
            this.lblNumberOfExtensions = new System.Windows.Forms.Label();
            this.btnClearExtensions = new System.Windows.Forms.Button();
            this.chkCombinePolicies = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(145, 55);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(611, 271);
            this.txtResults.TabIndex = 5;
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(8, 243);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(54, 17);
            this.chkTrace.TabIndex = 8;
            this.chkTrace.Text = "Trace";
            this.chkTrace.UseVisualStyleBackColor = true;
            // 
            // chkReturnPolicy
            // 
            this.chkReturnPolicy.AutoSize = true;
            this.chkReturnPolicy.Location = new System.Drawing.Point(8, 220);
            this.chkReturnPolicy.Name = "chkReturnPolicy";
            this.chkReturnPolicy.Size = new System.Drawing.Size(122, 17);
            this.chkReturnPolicy.TabIndex = 9;
            this.chkReturnPolicy.Text = "Return Policy ID List";
            this.chkReturnPolicy.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(766, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // requestToolStripMenuItem
            // 
            this.requestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendSingleRequestToolStripMenuItem,
            this.sendMultipleRequestToolStripMenuItem,
            this.openRequestGeneratorToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.requestToolStripMenuItem.Name = "requestToolStripMenuItem";
            this.requestToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.requestToolStripMenuItem.Text = "Request";
            // 
            // sendSingleRequestToolStripMenuItem
            // 
            this.sendSingleRequestToolStripMenuItem.Name = "sendSingleRequestToolStripMenuItem";
            this.sendSingleRequestToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.sendSingleRequestToolStripMenuItem.Text = "Send Single Request";
            this.sendSingleRequestToolStripMenuItem.Click += new System.EventHandler(this.button1_Click);
            // 
            // sendMultipleRequestToolStripMenuItem
            // 
            this.sendMultipleRequestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestsToolStripMenuItem,
            this.requestsToolStripMenuItem1,
            this.requestsToolStripMenuItem2,
            this.requestsToolStripMenuItem3,
            this.requestsToolStripMenuItem4});
            this.sendMultipleRequestToolStripMenuItem.Name = "sendMultipleRequestToolStripMenuItem";
            this.sendMultipleRequestToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.sendMultipleRequestToolStripMenuItem.Text = "Send Multiple Request";
            // 
            // requestsToolStripMenuItem
            // 
            this.requestsToolStripMenuItem.Name = "requestsToolStripMenuItem";
            this.requestsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.requestsToolStripMenuItem.Text = "2 Requests";
            this.requestsToolStripMenuItem.Click += new System.EventHandler(this.requestsToolStripMenuItem_Click);
            // 
            // requestsToolStripMenuItem1
            // 
            this.requestsToolStripMenuItem1.Name = "requestsToolStripMenuItem1";
            this.requestsToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.requestsToolStripMenuItem1.Text = "5 Requests";
            this.requestsToolStripMenuItem1.Click += new System.EventHandler(this.requestsToolStripMenuItem1_Click);
            // 
            // requestsToolStripMenuItem2
            // 
            this.requestsToolStripMenuItem2.Name = "requestsToolStripMenuItem2";
            this.requestsToolStripMenuItem2.Size = new System.Drawing.Size(154, 22);
            this.requestsToolStripMenuItem2.Text = "100 Requests";
            this.requestsToolStripMenuItem2.Click += new System.EventHandler(this.requestsToolStripMenuItem2_Click);
            // 
            // requestsToolStripMenuItem3
            // 
            this.requestsToolStripMenuItem3.Name = "requestsToolStripMenuItem3";
            this.requestsToolStripMenuItem3.Size = new System.Drawing.Size(154, 22);
            this.requestsToolStripMenuItem3.Text = "1000 Requests";
            this.requestsToolStripMenuItem3.Click += new System.EventHandler(this.requestsToolStripMenuItem3_Click);
            // 
            // requestsToolStripMenuItem4
            // 
            this.requestsToolStripMenuItem4.Name = "requestsToolStripMenuItem4";
            this.requestsToolStripMenuItem4.Size = new System.Drawing.Size(154, 22);
            this.requestsToolStripMenuItem4.Text = "10000 Requests";
            this.requestsToolStripMenuItem4.Click += new System.EventHandler(this.requestsToolStripMenuItem4_Click);
            // 
            // openRequestGeneratorToolStripMenuItem
            // 
            this.openRequestGeneratorToolStripMenuItem.Name = "openRequestGeneratorToolStripMenuItem";
            this.openRequestGeneratorToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openRequestGeneratorToolStripMenuItem.Text = "Open Request Generator";
            this.openRequestGeneratorToolStripMenuItem.Click += new System.EventHandler(this.openRequestGeneratorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // chkVerify
            // 
            this.chkVerify.AutoSize = true;
            this.chkVerify.Location = new System.Drawing.Point(8, 267);
            this.chkVerify.Name = "chkVerify";
            this.chkVerify.Size = new System.Drawing.Size(103, 17);
            this.chkVerify.TabIndex = 13;
            this.chkVerify.Text = "Verify Response";
            this.chkVerify.UseVisualStyleBackColor = true;
            // 
            // cmbComType
            // 
            this.cmbComType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComType.FormattingEnabled = true;
            this.cmbComType.Items.AddRange(new object[] {
            "XML over SOAP",
            "XML over REST",
            "JSON over REST"});
            this.cmbComType.Location = new System.Drawing.Point(8, 156);
            this.cmbComType.Name = "cmbComType";
            this.cmbComType.Size = new System.Drawing.Size(121, 21);
            this.cmbComType.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Communication Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Host name:";
            // 
            // txtPdpUrl
            // 
            this.txtPdpUrl.Location = new System.Drawing.Point(9, 55);
            this.txtPdpUrl.Name = "txtPdpUrl";
            this.txtPdpUrl.Size = new System.Drawing.Size(120, 20);
            this.txtPdpUrl.TabIndex = 17;
            this.txtPdpUrl.Text = "http://localhost:6009";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 513);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Client Certificate";
            // 
            // cmbCerts
            // 
            this.cmbCerts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCerts.FormattingEnabled = true;
            this.cmbCerts.Location = new System.Drawing.Point(16, 530);
            this.cmbCerts.Name = "cmbCerts";
            this.cmbCerts.Size = new System.Drawing.Size(739, 21);
            this.cmbCerts.TabIndex = 22;
            // 
            // txtDN
            // 
            this.txtDN.Location = new System.Drawing.Point(16, 559);
            this.txtDN.Margin = new System.Windows.Forms.Padding(2);
            this.txtDN.Name = "txtDN";
            this.txtDN.Size = new System.Drawing.Size(739, 20);
            this.txtDN.TabIndex = 23;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(15, 580);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(2);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(69, 24);
            this.btnConvert.TabIndex = 24;
            this.btnConvert.Text = "convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // txtConvDn
            // 
            this.txtConvDn.Location = new System.Drawing.Point(15, 608);
            this.txtConvDn.Margin = new System.Windows.Forms.Padding(2);
            this.txtConvDn.Name = "txtConvDn";
            this.txtConvDn.Size = new System.Drawing.Size(741, 20);
            this.txtConvDn.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Connector Type";
            // 
            // cmbConnectorType
            // 
            this.cmbConnectorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConnectorType.FormattingEnabled = true;
            this.cmbConnectorType.Items.AddRange(new object[] {
            "Anonymous",
            "WSSE",
            "XML Signing",
            "Client SSL"});
            this.cmbConnectorType.Location = new System.Drawing.Point(7, 103);
            this.cmbConnectorType.Name = "cmbConnectorType";
            this.cmbConnectorType.Size = new System.Drawing.Size(121, 21);
            this.cmbConnectorType.TabIndex = 26;
            // 
            // chkCertChain
            // 
            this.chkCertChain.AutoSize = true;
            this.chkCertChain.Location = new System.Drawing.Point(7, 196);
            this.chkCertChain.Margin = new System.Windows.Forms.Padding(2);
            this.chkCertChain.Name = "chkCertChain";
            this.chkCertChain.Size = new System.Drawing.Size(141, 17);
            this.chkCertChain.TabIndex = 29;
            this.chkCertChain.Text = "Include Certificate Chain";
            this.chkCertChain.UseVisualStyleBackColor = true;
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(8, 368);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(361, 132);
            this.txtContent.TabIndex = 30;
            this.txtContent.Text = resources.GetString("txtContent.Text");
            this.txtContent.WordWrap = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 352);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "XML Content";
            // 
            // lblExtension
            // 
            this.lblExtension.AutoSize = true;
            this.lblExtension.Location = new System.Drawing.Point(389, 352);
            this.lblExtension.Name = "lblExtension";
            this.lblExtension.Size = new System.Drawing.Size(133, 13);
            this.lblExtension.TabIndex = 33;
            this.lblExtension.Text = "XACML Extension Element";
            // 
            // txtXacmlExtension
            // 
            this.txtXacmlExtension.Location = new System.Drawing.Point(392, 368);
            this.txtXacmlExtension.Multiline = true;
            this.txtXacmlExtension.Name = "txtXacmlExtension";
            this.txtXacmlExtension.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtXacmlExtension.Size = new System.Drawing.Size(364, 132);
            this.txtXacmlExtension.TabIndex = 32;
            this.txtXacmlExtension.Text = resources.GetString("txtXacmlExtension.Text");
            this.txtXacmlExtension.WordWrap = false;
            // 
            // btnAddExtension
            // 
            this.btnAddExtension.Location = new System.Drawing.Point(568, 501);
            this.btnAddExtension.Name = "btnAddExtension";
            this.btnAddExtension.Size = new System.Drawing.Size(85, 23);
            this.btnAddExtension.TabIndex = 34;
            this.btnAddExtension.Text = "Add Extension";
            this.btnAddExtension.UseVisualStyleBackColor = true;
            this.btnAddExtension.Click += new System.EventHandler(this.btnAddExtension_Click);
            // 
            // lblNumberOf
            // 
            this.lblNumberOf.AutoSize = true;
            this.lblNumberOf.Location = new System.Drawing.Point(580, 352);
            this.lblNumberOf.Name = "lblNumberOf";
            this.lblNumberOf.Size = new System.Drawing.Size(145, 13);
            this.lblNumberOf.TabIndex = 35;
            this.lblNumberOf.Text = "Number of added extensions:";
            // 
            // lblNumberOfExtensions
            // 
            this.lblNumberOfExtensions.AutoSize = true;
            this.lblNumberOfExtensions.Location = new System.Drawing.Point(741, 352);
            this.lblNumberOfExtensions.Name = "lblNumberOfExtensions";
            this.lblNumberOfExtensions.Size = new System.Drawing.Size(13, 13);
            this.lblNumberOfExtensions.TabIndex = 36;
            this.lblNumberOfExtensions.Text = "0";
            // 
            // btnClearExtensions
            // 
            this.btnClearExtensions.Location = new System.Drawing.Point(659, 501);
            this.btnClearExtensions.Name = "btnClearExtensions";
            this.btnClearExtensions.Size = new System.Drawing.Size(97, 23);
            this.btnClearExtensions.TabIndex = 37;
            this.btnClearExtensions.Text = "Clear Extensions";
            this.btnClearExtensions.UseVisualStyleBackColor = true;
            this.btnClearExtensions.Click += new System.EventHandler(this.btnClearExtensions_Click);
            // 
            // chkCombinePolicies
            // 
            this.chkCombinePolicies.AutoSize = true;
            this.chkCombinePolicies.Checked = true;
            this.chkCombinePolicies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCombinePolicies.Location = new System.Drawing.Point(9, 290);
            this.chkCombinePolicies.Name = "chkCombinePolicies";
            this.chkCombinePolicies.Size = new System.Drawing.Size(106, 17);
            this.chkCombinePolicies.TabIndex = 38;
            this.chkCombinePolicies.Text = "Combine Policies";
            this.chkCombinePolicies.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 638);
            this.Controls.Add(this.chkCombinePolicies);
            this.Controls.Add(this.btnClearExtensions);
            this.Controls.Add(this.lblNumberOfExtensions);
            this.Controls.Add(this.lblNumberOf);
            this.Controls.Add(this.btnAddExtension);
            this.Controls.Add(this.lblExtension);
            this.Controls.Add(this.txtXacmlExtension);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.chkCertChain);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbConnectorType);
            this.Controls.Add(this.txtConvDn);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.txtDN);
            this.Controls.Add(this.cmbCerts);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPdpUrl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbComType);
            this.Controls.Add(this.chkVerify);
            this.Controls.Add(this.chkReturnPolicy);
            this.Controls.Add(this.chkTrace);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.CheckBox chkReturnPolicy;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem requestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendSingleRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendMultipleRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRequestGeneratorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkVerify;
        private System.Windows.Forms.ComboBox cmbComType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPdpUrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbCerts;
        private System.Windows.Forms.TextBox txtDN;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.TextBox txtConvDn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbConnectorType;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem4;
        private System.Windows.Forms.CheckBox chkCertChain;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.TextBox txtXacmlExtension;
        private System.Windows.Forms.Button btnAddExtension;
        private System.Windows.Forms.Label lblNumberOf;
        private System.Windows.Forms.Label lblNumberOfExtensions;
        private System.Windows.Forms.Button btnClearExtensions;
        private System.Windows.Forms.CheckBox chkCombinePolicies;
    }
}

