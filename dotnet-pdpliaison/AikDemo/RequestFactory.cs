using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PdpLiaison;

namespace AikDemo
{
    /// <summary>
    /// This form is used to create AuthorizationRequest objects.
    /// </summary>
    public partial class RequestFactory : Form
    {
        // The PdpConnector object to be used to create and send authorization requests.
        private PdpConnector pdpCon;

        public RequestFactory(PdpConnector pdpCon)
        {
            InitializeComponent();
            this.pdpCon = pdpCon;
        }

        private void RequestFactory_Load(object sender, EventArgs e)
        {
            lblPdpCon.Text = "Ask for an AuthorizationRequest instance from the " + this.Text;
        }

        private void btnCreateRequest_Click(object sender, EventArgs e)
        {
            //Instantiating an AuthorizationRequest using the PdpConnector.
            AuthorizationRequest req = pdpCon.createRequest(chkTrace.Checked, chkReturnPolicyIdList.Checked);

            //Sending the PdpConnector and the AuthorizatinRequest objects to the RequestFrm Form.
            RequestFrm reqFrm = new RequestFrm(pdpCon, req);
            reqFrm.Text = "AuthorizationRequest instantiated from the " + this.Text;
            reqFrm.Show();
        }
    }
}
