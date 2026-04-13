using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PdpLiaison;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace TestProject
{
    public partial class Form1 : Form
    {
        private List<XmlElement> extensions = new List<XmlElement>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PdpConnector pdpCon = new AnonymousConnector();
            Uri pdpUrl = new Uri(txtPdpUrl.Text);
            X509Store trustStore = new X509Store(StoreName.AddressBook, StoreLocation.CurrentUser);
            X509ChainPolicy chainPolicy = new X509ChainPolicy();
            chainPolicy.VerificationFlags = X509VerificationFlags.IgnoreEndRevocationUnknown;
            CertificateInclusion ci = CertificateInclusion.signingCertOnly;
            if (chkCertChain.Checked == true) {
                ci = CertificateInclusion.certificateChain;
            }
            CommunicationType ct = CommunicationType.XML_SOAP;

            switch (cmbComType.SelectedIndex) {
                case 1:
                    ct = CommunicationType.XML_REST;
                    break;
                case 2:
                    ct = CommunicationType.JSON_REST;
                    break;
                default:
                    ct= CommunicationType.XML_SOAP;
                    break;
            }
            
            switch (cmbConnectorType.SelectedIndex) {
                case 1:
                    //pdpCon = new WsseConnector();
                    break;
                case 2:
                    pdpCon = new XmlSigningConnector(pdpUrl, ct,
                        (X509Certificate2)cmbCerts.SelectedItem, ci, trustStore, chkVerify.Checked, chainPolicy);
                    break;
                case 3:
                    pdpCon = new ClientSslConnector(pdpUrl,
                        (X509Certificate2)cmbCerts.SelectedItem, trustStore, chkVerify.Checked, ct, chainPolicy);
                    break;
                default:
                    pdpCon = new AnonymousConnector(
                        pdpUrl, ct, trustStore, chkVerify.Checked, chainPolicy);
                    break;
            }

            pdpCon.registerObligation("email");
            pdpCon.registerObligation("log");
            AuthorizationRequest req = pdpCon.createRequest(chkTrace.Checked, chkReturnPolicy.Checked, chkCombinePolicies.Checked);
            req.addElement(
                AttributeCategory.access_subject,
                SubjectAttributes.subject_id,
                AttributeDataType._string,
                "asherma");
            //req.addElement(AttributeCategory.access_subject,
            //    "http://viewds.com/xacml/subject/rank",
            //    AttributeDataType._string,
            //    "Colonel");
            //req.addElement(AttributeCategory.access_subject,
            //    "http://viewds.com/xacml/subject/rank",
            //    AttributeDataType._string,
            //    "Private");
            req.addElement(
                AttributeCategory.resource,
                ResourceAttributes.resource_id,
                AttributeDataType._string,
                "REPORT B");
            //req.addElement(
            //    "MyCategory 1",
            //    "XYZ",
            //    AttributeDataType.integer,
            //    "44");
            //req.addElement(
            //    AttributeCategory.environment,
            //    "XYZ",
            //    AttributeDataType._string,
            //    "44");

            //req.addElement(
            //    "MyCategory 1",
            //    "QWE",
            //    AttributeDataType.boolean,
            //    "true");
            //req.addElement(
            //    "MyCategory 2",
            //    "ABC",
            //    AttributeDataType._string,
            //    "red");
            //req.addElement(
            //    "MyCategory 2",
            //    "ABC",
            //    AttributeDataType._string,
            //    "blue");

            if (!string.IsNullOrEmpty(txtContent.Text)) {
                System.Xml.XmlDocument contentDoc = new System.Xml.XmlDocument();
                contentDoc.LoadXml(txtContent.Text);
                req.setContent(AttributeCategory.resource, contentDoc.DocumentElement);
            }

            foreach (XmlElement ext in extensions) {
                req.addElement(ext);
            }

            try {
                AuthorizationResponse response = pdpCon.evaluate(req);
                switch (response.result) {
                    case Result.deny:
                        //deny
                        break;
                    case Result.denyWithObligations:
                        //deny
                        fulfilObligations(response.obligations);
                        break;
                    case Result.denyDueToUnrecognizedObligations:
                        //deny
                        break;
                    case Result.denyUnlessAllObligationsSatisfied:
                        if (fulfilObligations(response.obligations)) {
                            //permit
                        }
                        else {
                            //deny
                        }
                        break;
                    case Result.permit:
                        //permit
                        break;
                }

                txtResults.Text = response.ToString();
            }
            catch (Exception ex) {
                txtResults.Text = ex.Message;
            }
        }

        #region Obligations

        private bool fulfilObligations(List<Obligation> obligations)
        {
            foreach (Obligation ob in obligations) {
                if (ob.id == "Email") {
                    sendEmail(ob);
                }
                else {
                    return false;
                }
            }
            return true;
        }

        private bool ob1(Obligation ob)
        {
            return true;
        }

        private bool ob2(Obligation ob)
        {
            return true;
        }

        private bool sendEmail(Obligation ob)
        {
            MailMessage message = new MailMessage();
            List<string> recipientAddresses = new List<string>();

            message.From = new MailAddress("viewdsiispep@gmail.com", "ViewDS-IIS-PEP");

            foreach (AttributeAssignment aa in ob.attributes) {
                string attId = "";
                string attCat = "";
                string attVal = "";

                try { attId = aa.attributeId.ToLower(); }
                catch { }
                try { attCat = aa.categoryId.ToLower(); }
                catch { }
                try { attVal = aa.attributeValue; }
                catch { }

                if (attCat == "email" && attId == "recipientaddress") {
                    recipientAddresses.Add(attVal);
                }
                if (attCat == "email" && attId == "subject") {
                    message.Subject = attVal;
                }
                if (attCat == "email" && attId == "body") {
                    message.Body = attVal;
                }
            }

            if (recipientAddresses.Count < 1) {
                return false;
            }
            foreach (string recipient in recipientAddresses) {
                message.To.Add(recipient);
            }

            try {
                SmtpClient smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("viewdsiispep@gmail.com", "viewdsiispep")
                };
                //Uncomment this line to send the email.
                //smtp.Send(message);
            }
            catch {
                return false;
            }

            return true;
        }

        #endregion

        private void sendMultiReq(int numberOfRequests)
        {
            PdpConnector pdpCon = new AnonymousConnector();
            Uri pdpUrl = new Uri(txtPdpUrl.Text);
            X509Store trustStore = new X509Store(StoreName.AddressBook, StoreLocation.CurrentUser);
            X509ChainPolicy chainPolicy = new X509ChainPolicy();
            chainPolicy.RevocationMode = X509RevocationMode.Offline;
            chainPolicy.VerificationFlags = X509VerificationFlags.IgnoreEndRevocationUnknown;
            CertificateInclusion ci = CertificateInclusion.signingCertOnly;
            if (chkCertChain.Checked == true) {
                ci = CertificateInclusion.certificateChain;
            }
            CommunicationType ct = CommunicationType.XML_SOAP;

            switch (cmbComType.SelectedIndex) {
                case 1:
                    ct = CommunicationType.XML_REST;
                    break;
                case 2:
                    ct = CommunicationType.JSON_REST;
                    break;
                default:
                    ct = CommunicationType.XML_SOAP;
                    break;
            }

            switch (cmbConnectorType.SelectedIndex) {
                case 1:
                    //pdpCon = new WsseConnector();
                    break;
                case 2:
                    pdpCon = new XmlSigningConnector(pdpUrl, ct, (X509Certificate2)cmbCerts.SelectedItem,
                        ci, trustStore, chkVerify.Checked, chainPolicy);
                    break;
                case 3:
                    pdpCon = new ClientSslConnector(pdpUrl,
                        (X509Certificate2)cmbCerts.SelectedItem, trustStore, chkVerify.Checked, ct, chainPolicy);
                    break;
                default:
                    pdpCon = new AnonymousConnector(pdpUrl, ct, trustStore, chkVerify.Checked, chainPolicy);
                    break;
            }

            pdpCon.registerObligation("email");
            MultiRequest mulReq = createMultiRequest(pdpCon, numberOfRequests);
            try {
                MultiResponse mulRes = pdpCon.evaluate(mulReq);
                //Show the response.
                string comprehensive = mulRes.ToString();
                comprehensive += "\r\n\r\n\r\n================FINISH==============";
                foreach (AuthorizationRequest req in mulReq.requests) {
                    AuthorizationResponse resElem = mulRes.getResponseForRequest(req);
                    comprehensive += "\r\nRequest: " + req.uid.ToString();
                    comprehensive += "\r\nResposnse Element\r\n" + resElem.ToString();
                }
                txtResults.Text = comprehensive;
            }
            catch (Exception ex) {
                txtResults.Text = ex.Message;
            }
        }

        private MultiRequest createMultiRequest(PdpConnector pdpCon, int numberOfRequests)
        {
            MultiRequest mulReq = new MultiRequest(chkReturnPolicy.Checked);

            for (int i = 0; i < numberOfRequests - 1; i++) {
                AuthorizationRequest req1 = pdpCon.createRequest(chkTrace.Checked);
                req1.addElement(AttributeCategory.access_subject,
                    SubjectAttributes.role,
                    AttributeDataType._string,
                    "MANAGER");
                req1.addElement(AttributeCategory.resource,
                    ResourceAttributes.resource_id,
                    AttributeDataType._string,
                    "REPORT " + i.ToString());
                mulReq.addRequest(req1);
            }

            AuthorizationRequest req2 = pdpCon.createRequest(false, chkReturnPolicy.Checked, chkCombinePolicies.Checked);
            req2.addElement(AttributeCategory.access_subject,
                SubjectAttributes.role,
                AttributeDataType._string,
                "MANAGER");
            req2.addElement(AttributeCategory.resource,
                ResourceAttributes.resource_id,
                AttributeDataType._string,
                "REPORT B");
            req2.addElement(AttributeCategory.environment,
                EnvironmentAttributes.current_dateTime,
                AttributeDataType.dateTime,
                PdpConnector.formatGlobalTimeForXml(DateTime.Now));

            foreach (XmlElement ext in extensions) {
                req2.addElement(ext);
            }

            mulReq.addRequest(req2);

            foreach (XmlElement ext in extensions) {
                mulReq.addElement(ext);
            }

            return mulReq;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openRequestGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestGenerator reqGenFrm = new RequestGenerator();
            reqGenFrm.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbConnectorType.SelectedIndex = 0;
            cmbComType.SelectedIndex = 0;
            populateListView();
        }

        private void populateListView()
        {
            X509Store store = new X509Store("My");
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 mCert in store.Certificates) {
                cmbCerts.Items.Add(mCert);
            }
            if (cmbCerts.Items.Count > 0) {
                cmbCerts.SelectedIndex = cmbCerts.Items.Count - 1;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            //DistinguishedName dn = new DistinguishedName(txtDN.Text);
            //txtConvDn.Text = dn.x500dn().Name;
        }

        private void requestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sendMultiReq(2);
        }

        private void requestsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sendMultiReq(5);
        }

        private void requestsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            sendMultiReq(100);
        }

        private void requestsToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            sendMultiReq(1000);
        }

        private void requestsToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            sendMultiReq(10000);
        }

        private void btnAddExtension_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtXacmlExtension.Text)) {
                System.Xml.XmlDocument extensionDoc = new System.Xml.XmlDocument();
                extensionDoc.LoadXml(txtXacmlExtension.Text);
                extensions.Add(extensionDoc.DocumentElement);
            }

            lblNumberOfExtensions.Text = extensions.Count.ToString();
        }

        private void btnClearExtensions_Click(object sender, EventArgs e)
        {
            extensions.Clear();
        }
    }
}
