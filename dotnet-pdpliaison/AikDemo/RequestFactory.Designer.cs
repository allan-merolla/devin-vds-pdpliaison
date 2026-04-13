namespace AikDemo
{
    partial class RequestFactory
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
            this.lblPdpCon = new System.Windows.Forms.Label();
            this.btnCreateRequest = new System.Windows.Forms.Button();
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.chkReturnPolicyIdList = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblPdpCon
            // 
            this.lblPdpCon.Location = new System.Drawing.Point(26, 27);
            this.lblPdpCon.Name = "lblPdpCon";
            this.lblPdpCon.Size = new System.Drawing.Size(317, 60);
            this.lblPdpCon.TabIndex = 0;
            this.lblPdpCon.Text = "lblPdpCon";
            // 
            // btnCreateRequest
            // 
            this.btnCreateRequest.Location = new System.Drawing.Point(70, 123);
            this.btnCreateRequest.Name = "btnCreateRequest";
            this.btnCreateRequest.Size = new System.Drawing.Size(238, 23);
            this.btnCreateRequest.TabIndex = 1;
            this.btnCreateRequest.Text = "Instantiate an AuthorizationRequest object";
            this.btnCreateRequest.UseVisualStyleBackColor = true;
            this.btnCreateRequest.Click += new System.EventHandler(this.btnCreateRequest_Click);
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(70, 77);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(54, 17);
            this.chkTrace.TabIndex = 2;
            this.chkTrace.Text = "Trace";
            this.chkTrace.UseVisualStyleBackColor = true;
            // 
            // chkReturnPolicyIdList
            // 
            this.chkReturnPolicyIdList.AutoSize = true;
            this.chkReturnPolicyIdList.Location = new System.Drawing.Point(70, 100);
            this.chkReturnPolicyIdList.Name = "chkReturnPolicyIdList";
            this.chkReturnPolicyIdList.Size = new System.Drawing.Size(122, 17);
            this.chkReturnPolicyIdList.TabIndex = 3;
            this.chkReturnPolicyIdList.Text = "Return Policy ID List";
            this.chkReturnPolicyIdList.UseVisualStyleBackColor = true;
            // 
            // RequestFactory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 158);
            this.Controls.Add(this.chkReturnPolicyIdList);
            this.Controls.Add(this.chkTrace);
            this.Controls.Add(this.btnCreateRequest);
            this.Controls.Add(this.lblPdpCon);
            this.MaximizeBox = false;
            this.Name = "RequestFactory";
            this.Text = "Request Factory";
            this.Load += new System.EventHandler(this.RequestFactory_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPdpCon;
        private System.Windows.Forms.Button btnCreateRequest;
        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.CheckBox chkReturnPolicyIdList;
    }
}