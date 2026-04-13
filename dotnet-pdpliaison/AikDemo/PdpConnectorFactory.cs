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
    /// This form is used to create PdpConnector objects.
    /// </summary>
    public partial class PdpConnectorFactory : Form
    {
        // The number of the connectors
        int connectorNumber = 0;

        public PdpConnectorFactory()
        {
            InitializeComponent();
        }

        private void btnBuildConnector_Click(object sender, EventArgs e)
        {
            try {
                Uri pdpUrl = new Uri(txtPdpUrl.Text);
                //Instantiating a PdpConnector object
                PdpConnector pdpCon = new AnonymousConnector(pdpUrl, CommunicationType.JSON_REST, null, false);
                connectorNumber++;

                //Sending the PdpConnector object to the RequestFactory form
                RequestFactory reqFactory = new RequestFactory(pdpCon);
                reqFactory.Text = "Connector #" + connectorNumber.ToString() +
                    " (" + pdpUrl.OriginalString + ")";
                reqFactory.Show();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
