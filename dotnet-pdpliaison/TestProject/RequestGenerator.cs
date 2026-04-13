using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using PdpLiaison;

namespace TestProject
{
    public partial class RequestGenerator : Form
    {
        PdpConnector pdpCon;

        public RequestGenerator()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
                return;
            }

            //Reading XML File
            Random rnd = new Random(DateTime.Now.Millisecond);
            string fileName = openFileDialog1.FileName;
            XDocument xdoc = XDocument.Load(fileName);
            var cats = from lv1 in xdoc.Descendants("Attributes")
                       select new {
                           name = lv1.Attribute("Category").Value,
                           children = lv1.Descendants("Attribute")
                       };

            //Creating PDP Connector
            pdpCon = new AnonymousConnector(new Uri(txtHostName.Text), CommunicationType.XML_SOAP, null, false);

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000; i++) {
                //Creating an XACML Request
                AuthorizationRequest req = pdpCon.createRequest();

                foreach (var cat in cats) {
                    foreach (var att in cat.children) {
                        XElement[] values = att.Descendants("AttributeValue").ToArray();
                        XElement randomValue = values[rnd.Next(values.Count())];

                        string catName = cat.name;
                        string attName = att.Attribute("AttributeId").Value;
                        string dataType = randomValue.Attribute("DataType").Value;
                        string val = randomValue.Value;
                        req.addElement(catName, attName, dataType, val);
                    }
                }

                AuthorizationResponse res = pdpCon.evaluate(req);
            }

            sw.Stop();

            TimeSpan ts = sw.Elapsed;

            MessageBox.Show(ts.ToString());
        }
    }
}
