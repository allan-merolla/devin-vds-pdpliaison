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
    /// This form is used to add XACML attributes to the AuthorizationRequest and send it to the PDP.
    /// </summary>
    public partial class RequestFrm : Form
    {
        // The PdpConnector object to be used to send the authorization requests to the PDP.
        private PdpConnector pdpCon;

        //The AuthorizationRequest object to be sent to the PDP.
        private AuthorizationRequest req;

        public RequestFrm(PdpConnector pdpCon, AuthorizationRequest req)
        {
            InitializeComponent();
            this.pdpCon = pdpCon;
            this.req = req;
        }

        private void btnAddAttribute_Click(object sender, EventArgs e)
        {
            AttributeDataType dt = (AttributeDataType)cmbDataType.SelectedItem;

            //Adding the XACML attribute to the request.
            req.addElement(cmbCategory.Text, cmbIdentifier.Text, dt, txtValue.Text);

            //Adding the XACML attribute to the data grid view.
            dgvAttributes.Rows.Add(cmbCategory.Text, cmbIdentifier.Text, cmbDataType.Text, txtValue.Text);
        }

        /// <summary>
        /// This is to show the text of the request in a separate form. 
        /// This is not a necessary step in the request evaluation process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRequestText_Click(object sender, EventArgs e)
        {
            //Making the XML text of the request easier to read (indented format).
            string strRequest = "";
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
            System.Xml.XmlTextWriter xtw = null;
            try {
                xtw = new System.Xml.XmlTextWriter(sw);
                xtw.Formatting = System.Xml.Formatting.Indented;
                req.xmlDoc.WriteTo(xtw);
                strRequest = sb.ToString();
                Results resFrm = new Results();
                resFrm.Text = "Request Text";
                resFrm.showText(strRequest);
                resFrm.Show();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            finally {
                if (xtw != null) {
                    xtw.Close();
                }
            }
        }

        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            //Open the Results form to show the response received from the PDP.
            Results resFrm = new Results();
            resFrm.Show();

            try {
                //Send the request to the PDP.
                AuthorizationResponse res = pdpCon.evaluate(req);
                //Show the response.
                resFrm.Text = "Response Text";
                resFrm.showText(res.ToString());
                //Assessing the response.
                if (res.result == Result.permit) {
                    MessageBox.Show("Permitted", "Access");
                }
                else {
                    MessageBox.Show("Not permitted", "Access");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                resFrm.Text = "Response Text";
            }
        }

        #region UI controls

        private void RequestFrm_Load(object sender, EventArgs e)
        {
            populateCategories();
            populateDataType();
        }

        private void populateDataType()
        {
            cmbDataType.DataSource = System.Enum.GetValues(typeof(AttributeDataType));
            cmbDataType.SelectedItem = AttributeDataType._string;
        }

        private void cmbDesignatorCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateIdentifiers();
        }

        private void populateCategories()
        {
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:1.0:subject-category:access-subject");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:3.0:attribute-category:action");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:1.0:subject-category:codebase");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:3.0:attribute-category:delegate");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:3.0:attribute-category:delegation-info");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:3.0:attribute-category:environment");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:1.0:subject-category:intermediary-subject");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:1.0:subject-category:recipient-subject");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:1.0:subject-category:requesting-machine");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:3.0:attribute-category:resource");
            cmbCategory.Items.Add("urn:oasis:names:tc:xacml:2.0:subject-category:role-enablement-authority");

            cmbCategory.Sorted = true;
        }

        private void populateIdentifiers()
        {
            cmbIdentifier.Items.Clear();

            switch (cmbCategory.Text) {
                case "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject":
                case "urn:oasis:names:tc:xacml:1.0:subject-category:recipient-subject":
                case "urn:oasis:names:tc:xacml:1.0:subject-category:intermediary-subject":
                case "urn:oasis:names:tc:xacml:1.0:subject-category:codebase":
                case "urn:oasis:names:tc:xacml:1.0:subject-category:requesting-machine":
                case "urn:oasis:names:tc:xacml:2.0:subject-category:role-enablement-authority":
                case "urn:oasis:names:tc:xacml:3.0:attribute-category:delegate":
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:subject-id");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:subject-id-qualifier");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:key-info");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:authentication-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:authentication-method");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:request-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:session-start-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:3.0:subject:authn-locality:ip-address");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:3.0:subject:authn-locality:dns-name");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:2.0:subject:role");
                    break;
                case "urn:oasis:names:tc:xacml:3.0:attribute-category:action":
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:action:action-id");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:action:implied-action");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:action:action-namespace");
                    break;
                case "urn:oasis:names:tc:xacml:3.0:attribute-category:delegation-info":
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:3.0:delegation:decision");
                    break;
                case "urn:oasis:names:tc:xacml:3.0:attribute-category:environment":
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:environment:current-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:environment:current-date");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:environment:current-dateTime");
                    break;
                case "urn:oasis:names:tc:xacml:3.0:attribute-category:resource":
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:resource:resource-id");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:2.0:resource:target-namespace");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:resource:xpath");
                    break;
                default:
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:subject-id");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:subject-id-qualifier");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:key-info");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:authentication-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:authentication-method");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:request-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:subject:session-start-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:3.0:subject:authn-locality:ip-address");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:3.0:subject:authn-locality:dns-name");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:2.0:subject:role");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:action:action-id");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:action:implied-action");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:action:action-namespace");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:3.0:delegation:decision");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:environment:current-time");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:environment:current-date");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:environment:current-dateTime");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:resource:resource-id");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:2.0:resource:target-namespace");
                    cmbIdentifier.Items.Add("urn:oasis:names:tc:xacml:1.0:resource:xpath");
                    break;
            }

            cmbIdentifier.Sorted = true;

            if (cmbIdentifier.Items.Count > 0) {
                cmbIdentifier.SelectedIndex = 0;
            }
        }

        #endregion
    }
}
