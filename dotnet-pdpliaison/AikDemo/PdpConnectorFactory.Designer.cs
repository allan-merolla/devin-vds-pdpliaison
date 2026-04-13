namespace AikDemo
{
    partial class PdpConnectorFactory
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
            this.lblPdpUrl = new System.Windows.Forms.Label();
            this.txtPdpUrl = new System.Windows.Forms.TextBox();
            this.btnBuildConnector = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPdpUrl
            // 
            this.lblPdpUrl.AutoSize = true;
            this.lblPdpUrl.Location = new System.Drawing.Point(40, 70);
            this.lblPdpUrl.Name = "lblPdpUrl";
            this.lblPdpUrl.Size = new System.Drawing.Size(54, 13);
            this.lblPdpUrl.TabIndex = 0;
            this.lblPdpUrl.Text = "PDP URL";
            // 
            // txtPdpUrl
            // 
            this.txtPdpUrl.Location = new System.Drawing.Point(110, 70);
            this.txtPdpUrl.Name = "txtPdpUrl";
            this.txtPdpUrl.Size = new System.Drawing.Size(195, 20);
            this.txtPdpUrl.TabIndex = 1;
            this.txtPdpUrl.Text = "http://localhost:6009";
            // 
            // btnBuildConnector
            // 
            this.btnBuildConnector.Location = new System.Drawing.Point(43, 142);
            this.btnBuildConnector.Name = "btnBuildConnector";
            this.btnBuildConnector.Size = new System.Drawing.Size(262, 23);
            this.btnBuildConnector.TabIndex = 4;
            this.btnBuildConnector.Text = "Instantiate a PdpConnector object";
            this.btnBuildConnector.UseVisualStyleBackColor = true;
            this.btnBuildConnector.Click += new System.EventHandler(this.btnBuildConnector_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(13, 13);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(165, 13);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "Enter the URL of the PDP server:";
            // 
            // PdpConnectorFactory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 191);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnBuildConnector);
            this.Controls.Add(this.txtPdpUrl);
            this.Controls.Add(this.lblPdpUrl);
            this.MaximizeBox = false;
            this.Name = "PdpConnectorFactory";
            this.Text = "PDP Connector Factory";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPdpUrl;
        private System.Windows.Forms.TextBox txtPdpUrl;
        private System.Windows.Forms.Button btnBuildConnector;
        private System.Windows.Forms.Label lblMessage;
    }
}