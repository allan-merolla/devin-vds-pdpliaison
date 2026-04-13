using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Diagnostics;
using PdpLiaison.Exceptions;

namespace PdpLiaison
{
    /// <summary>
    /// PdpConnector
    /// </summary>
    public abstract class PdpConnector
    {
        private string soap_profile = namespaceURI.soap_1_2;

        private Uri pcPdpUrl;
        private CommunicationType pcCommunicationType;
        private X509Store pcTrustStore;
        private bool pcVerifySignature;
        private X509ChainPolicy pcChainPolicy;

        /// <summary>
        /// Certificate that the AIK uses to establish an SSL connection to the PDP
        /// </summary>
        protected X509Certificate2 pcClientSslCertificate;

        /// <summary>
        /// Obligations that can be fulfilled by the application.
        /// </summary>
        protected List<string> recognisedObligations;

        /// <summary>
        /// The method of the communication with the PDP.
        /// </summary>
        public CommunicationType communicationType
        {
            get { return pcCommunicationType; }
        }

        /// <summary>
        /// The signatures on the PDP responses will be checked if this is set to true.
        /// This flag can be set using the constructors only.
        /// </summary>
        public bool verifySignature
        {
            get { return pcVerifySignature; }
        }

        /// <summary>
        /// The AIK searches this certificate store to find the certificate that the server uses to 
        /// sign the responses when only the subject name of the signing certificate is added to the signature.
        /// </summary>
        public X509Store trustStore
        {
            get { return pcTrustStore; }
        }

        /// <summary>
        /// The URL of the PDP.
        /// This can be set using the constructors only.
        /// </summary>
        public Uri pdpUrl
        {
            get { return pcPdpUrl; }
        }

        /// <summary>
        /// The chain-building engine for certificate verification.
        /// </summary>
        public X509ChainPolicy chainPolicy
        {
            get { return pcChainPolicy; }
        }

        #region SAML Attributes

        private string arIssueInstant = "";
        private string arSamlStatus = "";
        private string arAuthzIssuer = "";
        private string arInResponseTo = "";

        /// <summary>
        /// string
        /// </summary>
        public string issueInstant
        {
            get { return arIssueInstant; }
        }

        /// <summary>
        /// string
        /// </summary>
        public string samlStatus
        {
            get { return arSamlStatus; }
        }

        /// <summary>
        /// string
        /// </summary>
        public string authzIssuer
        {
            get { return arAuthzIssuer; }
        }

        /// <summary>
        /// string
        /// </summary>
        public string inResponseTo
        {
            get { return arInResponseTo; }
        }

        private void parseXmlResponse(XmlDocument serverXmlResponse)
        {
            XmlNodeList nodeList;

            try {
                //Get InResponseTo and IssueInstant
                nodeList = serverXmlResponse.GetElementsByTagName("Response", namespaceURI.saml_protocol);
                if (nodeList.Count > 0) {
                    XmlNode samlResponse = nodeList[0];
                    arIssueInstant = samlResponse.Attributes["IssueInstant"].Value;
                    arInResponseTo = samlResponse.Attributes["InResponseTo"].Value;
                }

                //Saml Status
                nodeList = serverXmlResponse.GetElementsByTagName("StatusCode", namespaceURI.saml_protocol);
                if (nodeList.Count > 0) {
                    XmlNode statusCodeNode = nodeList[0];
                    arSamlStatus = statusCodeNode.Attributes["Value"].Value;
                }

                //Issuer
                nodeList = serverXmlResponse.GetElementsByTagName("Issuer", namespaceURI.saml_assertion);
                if (nodeList.Count > 0) {
                    XmlNode issuerNode = nodeList[0];
                    arAuthzIssuer = issuerNode.InnerText;
                }
            }
            catch (Exception ex) {
                throw new AikException("Error in parsing the XML response", ex);
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PdpConnector()
        {
            recognisedObligations = new List<string>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdpUrl">The URL of the PDP</param>
        /// <param name="communicationType">The type of communication with the PDP.</param>
        /// <param name="trustStore">The AIK searches this certificate store to find the certificate that the server has used to 
        /// sign the responses when only the subject name of the signing certificate is included in the signature.</param>
        /// <param name="verifySignature">The signatures on the PDP responses will be checked if this is set to true.</param>
        public PdpConnector(Uri pdpUrl, CommunicationType communicationType, X509Store trustStore, bool verifySignature)
        {
            if (verifySignature == true && communicationType == CommunicationType.JSON_REST) {
                throw new AikSecurityException("XML Signature is not supported in CommunicationType.JSON_REST.");
            }
            this.pcPdpUrl = pdpUrl;
            this.pcCommunicationType = communicationType;
            this.pcVerifySignature = verifySignature;
            this.pcTrustStore = trustStore;
            recognisedObligations = new List<string>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdpUrl">The URL of the PDP</param>
        /// <param name="communicationType">The type of communication with the PDP.</param>
        /// <param name="trustStore">The AIK searches this certificate store to find the certificate that the server has used to 
        /// sign the responses when only the subject name of the signing certificate is included in the signature.</param>
        /// <param name="verifySignature">The signatures on the PDP responses will be checked if this is set to true.</param>
        /// <param name="chainPolicy">The AIK, by default, builds a simple chain for the certificates and applies the 
        /// base policy to that chain. But if this parameter is set, then it will be applied to the chain.</param>
        public PdpConnector(Uri pdpUrl, CommunicationType communicationType, X509Store trustStore, bool verifySignature, X509ChainPolicy chainPolicy)
        {
            if (verifySignature == true && communicationType == CommunicationType.JSON_REST) {
                throw new AikSecurityException("XML Signature is not supported in CommunicationType.JSON_REST.");
            }
            this.pcPdpUrl = pdpUrl;
            this.pcCommunicationType = communicationType;
            this.pcVerifySignature = verifySignature;
            this.pcTrustStore = trustStore;
            this.pcChainPolicy = chainPolicy;
            recognisedObligations = new List<string>();
        }

        /// <summary>
        /// This method is used for registering the obligations that the calling application recognizes and can fulfill.
        /// </summary>
        /// <param name="obligationIdentifier"></param>
        public void registerObligation(string obligationIdentifier)
        {
            this.recognisedObligations.Add(obligationIdentifier);
        }

        /// <summary>
        /// Instantiates and returns an empty AuthorizationRequest object
        /// </summary>
        /// <returns>AuthorizationRequest</returns>
        public AuthorizationRequest createRequest()
        {
            return new AuthorizationRequest(false, false);
        }

        /// <summary>
        /// Instantiates and returns an empty AuthorizationRequest object
        /// </summary>
        /// <param name="trace">Trace information will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <returns>AuthorizationRequest</returns>
        public AuthorizationRequest createRequest(bool trace)
        {
            return new AuthorizationRequest(trace, false);
        }

        /// <summary>
        /// Instantiates and returns an empty AuthorizationRequest object
        /// </summary>
        /// <param name="trace">Trace information will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <param name="returnPolicyIdList">PolicyIdList will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <returns>AuthorizationRequest</returns>
        public AuthorizationRequest createRequest(bool trace, bool returnPolicyIdList)
        {
            return new AuthorizationRequest(trace, returnPolicyIdList);
        }

        /// <summary>
        /// Instantiates and returns an empty AuthorizationRequest object
        /// </summary>
        /// <param name="trace">Trace information will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <param name="returnPolicyIdList">PolicyIdList will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <param name="combinePolicies">PDP will combine its policies with the policies provided 
        /// in the request if this flag is set to true.</param>
        /// <returns>AuthorizationRequest</returns>
        public AuthorizationRequest createRequest(bool trace, bool returnPolicyIdList, bool combinePolicies)
        {
            return new AuthorizationRequest(trace, returnPolicyIdList, combinePolicies);
        }

        /// <summary>
        /// Sends the AuthorizationRequest to the PDP, receives and processes the response 
        /// from the PDP, and builds and returns an AuthorizationResponse object.
        /// </summary>
        /// <param name="request">AuthorizationRequest object to be evaluated.</param>
        /// <returns>AuthorizationResponse</returns>
        public virtual AuthorizationResponse evaluate(AuthorizationRequest request)
        {
            throw new AikException("Not implemented");
        }

        /// <summary>
        /// Sends the MultiRequest to the PDP, receives and processes the response 
        /// from the PDP, and builds and returns a MultiResponse object.
        /// </summary>
        /// <param name="multiReq">MultiRequest object to be evaluated</param>
        /// <returns>MultiResponse</returns>
        public virtual MultiResponse evaluate(MultiRequest multiReq)
        {
            throw new AikException("Not implemented");
        }

        /// <summary>
        /// Adds the SOAP wrapper to the XML doc and sends it to the server over HTTP(S), 
        /// and verifies the signature on the response if required.
        /// </summary>
        /// <param name="soapRequest">The XML format of the message to be sent.</param>
        /// <param name="indented">Formats the request before sending to the PDP. This should be set to false for signed messages.</param>
        /// <returns>The XML format of the response.</returns>
        protected XmlDocument sendXmlRequest(XmlDocument soapRequest, bool indented)
        {
            XmlNodeList nodeList;

            XmlDocument serverResponse = new XmlDocument();
            serverResponse.PreserveWhitespace = true;

            string strRequest = "";

            if (indented == true) {
                //Format the Xml text of the request
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                XmlTextWriter xtw = null;
                try {
                    xtw = new XmlTextWriter(sw);
                    xtw.Formatting = Formatting.Indented;
                    soapRequest.WriteTo(xtw);
                }
                finally {
                    if (xtw != null) {
                        xtw.Close();
                    }
                }
                strRequest = sb.ToString();
            }
            else {
                strRequest = soapRequest.OuterXml;
            }

            //Sending the request
            string httpRespond = httpPostXml(strRequest);
            serverResponse.LoadXml(httpRespond);

            if (verifySignature == true) {
                //Get the SAML element 
                nodeList = serverResponse.GetElementsByTagName("Response", namespaceURI.saml_protocol);
                if (nodeList.Count > 0) {
                    XmlElement samlResp = (XmlElement)nodeList[0];
                    verifyXmlSignature(samlResp);
                }
            }

            parseXmlResponse(serverResponse);

            //Check the inResponseTo against queryId
            string queryId = "";
            try {
                //Get InResponseTo and IssueInstant
                nodeList = soapRequest.GetElementsByTagName("XACMLAuthzDecisionQuery", namespaceURI.xacml_saml_schema);
                if (nodeList.Count > 0) {
                    XmlNode samlResponse = nodeList[0];
                    queryId = samlResponse.Attributes["ID"].Value;
                }
            }
            catch (Exception ex) {
                throw new AikException("Error in parsing the XML response", ex);
            }
            
            if (queryId != arInResponseTo) {
                throw new AikException("Request ID does not match the response ID.");
            }

            return serverResponse;
        }

        /// <summary>
        /// Adds the SAML wrapper to the XACML request.
        /// </summary>
        /// <param name="request">RequestBase</param>
        /// <returns>XmlDocument</returns>
        protected XmlDocument addSamlWrapper(RequestBase request)
        {
            XmlDocument samlDoc = new XmlDocument();
            samlDoc.PreserveWhitespace = true;
            XmlNode decisionQuery = samlDoc.CreateNode(XmlNodeType.Element, "xsp", "XACMLAuthzDecisionQuery",
                namespaceURI.xacml_saml_schema);

            XmlAttribute att2 = samlDoc.CreateAttribute("xmlns", "xsp", "http://www.w3.org/2000/xmlns/");
            att2.Value = namespaceURI.xacml_saml_schema;
            decisionQuery.Attributes.Append(att2);

            XmlAttribute att3 = samlDoc.CreateAttribute("ID");
            att3.Value = "_" + (Guid.NewGuid()).ToString();
            decisionQuery.Attributes.Append(att3);

            XmlAttribute att4 = samlDoc.CreateAttribute("Version");
            att4.Value = "2.0";
            decisionQuery.Attributes.Append(att4);

            XmlAttribute att5 = samlDoc.CreateAttribute("IssueInstant");
            att5.Value = PdpConnector.formatGlobalTimeForXml(DateTime.Now.ToUniversalTime());
            decisionQuery.Attributes.Append(att5);

            XmlAttribute att6 = samlDoc.CreateAttribute("CombinePolicies");
            att6.Value = request.combinePolicies.ToString().ToLower();
            decisionQuery.Attributes.Append(att6);

            decisionQuery.AppendChild(decisionQuery.OwnerDocument.ImportNode(request.xmlDoc.DocumentElement, true));
            samlDoc.AppendChild(decisionQuery);

            foreach (XmlElement extention in request.xacmlExtensions) {
                samlDoc.DocumentElement.AppendChild(samlDoc.ImportNode(extention, true));
            }

            return samlDoc;
        }

        /// <summary>
        /// Adds the SOAP envelope to the message
        /// </summary>
        /// <param name="xmlRequest">XmlDocument</param>
        /// <returns>XmlDocument</returns>
        protected XmlDocument addSoapWrapper(XmlDocument xmlRequest)
        {
            XmlDocument soapMessage = new XmlDocument();
            XmlElement soapEnvelope;

            //Adding soap envelope
            soapMessage.AppendChild(soapMessage.CreateXmlDeclaration("1.0", "UTF-8", ""));
            soapEnvelope = soapMessage.CreateElement("SOAP-ENV", "Envelope", soap_profile);

            XmlAttribute att0 = soapMessage.CreateAttribute("xmlns", "SOAP-ENV", "http://www.w3.org/2000/xmlns/");
            att0.Value = soap_profile;
            soapEnvelope.Attributes.Append(att0);

            soapMessage.AppendChild(soapEnvelope);

            //Adding soap header
            XmlNode soapHeader = soapMessage.CreateNode(XmlNodeType.Element, "SOAP-ENV", "Header", soap_profile);
            soapEnvelope.AppendChild(soapHeader);

            //Adding soap body
            XmlNode soapBody = soapMessage.CreateNode(XmlNodeType.Element, "SOAP-ENV", "Body", soap_profile);
            soapBody.AppendChild(soapBody.OwnerDocument.ImportNode(xmlRequest.DocumentElement, true));
            soapEnvelope.AppendChild(soapBody);

            return soapMessage;
        }

        private WebResponse httpPost(string parameters)
        {
            if (pdpUrl == null) {
                throw new AikException("Invalid PDP URL.");
            }
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(pdpUrl.AbsoluteUri);

            if (pdpUrl.AbsoluteUri.ToLower().StartsWith("https")) {
                ServicePointManager.ServerCertificateValidationCallback += CertificateValidationCallBack;
            }

            if (pcClientSslCertificate != null) {
                webRequest.ClientCertificates.Add(pcClientSslCertificate);
            }

            byte[] bytes = null;

            switch (pcCommunicationType) {
                case CommunicationType.XML_REST:
                    webRequest.ContentType = "application/xacml+xml; version=3.0";
                    bytes = Encoding.UTF8.GetBytes(parameters);
                    break;
                case CommunicationType.JSON_REST:
                    webRequest.ContentType = "application/xacml+json; version=3.0";
                    bytes = Encoding.UTF8.GetBytes(parameters);
                    break;
                default:
                    if (soap_profile == namespaceURI.soap_1_1) {
                        webRequest.ContentType = "text/xml";
                    }
                    else {
                        webRequest.ContentType = "application/soap+xml";
                    }
                    bytes = Encoding.ASCII.GetBytes(parameters);
                    break;
            }

            webRequest.Method = "POST";
            Stream os = null;
            try { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException ex) {
                throw new AikConnectionException("Error in sending request to PDP: " + ex.Message, ex);
            }
            finally {
                if (os != null) {
                    os.Close();
                }
            }

            WebResponse webResponse = null;

            try { // get the response
                webResponse = webRequest.GetResponse();
                if (webResponse == null) { return null; }
            }
            catch (WebException ex) {
                throw new AikConnectionException("Error in receiving response from PDP: " + ex.Message, ex);
            }

            return webResponse;
        }

        /// <summary>
        /// HTTP Post
        /// </summary>
        /// <param name="parameters">string</param>
        /// <returns>string</returns>
        private string httpPostXml(string parameters)
        {
            WebResponse webResponse = httpPost(parameters);

            if (webResponse == null) {
                return "";
            }

            try { // get the response
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex) {
                throw new AikConnectionException("Error in receiving response from PDP: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends the JSON request to the server using REST.
        /// </summary>
        /// <param name="jsonRequest">The JSON format of the message to be sent.</param>
        /// <returns>The JSON response converted to XElement</returns>
        protected XElement httpPostJson(string jsonRequest)
        {
            WebResponse serverJsonResponse = httpPost(jsonRequest);
            XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(serverJsonResponse.GetResponseStream(), XmlDictionaryReaderQuotas.Max);
            XElement responseObj = XElement.Load(reader);
            return responseObj.Element("Response");
        }

        protected XElement SingleResponse(XElement responseNode)
        {
            if (responseNode.Attribute("type").Value == "array") {
                foreach (var prElement in responseNode.Elements()) {
                    return prElement;
                }
                return null;
            }
            return responseNode;
        }

        private bool CertificateValidationCallBack(object sender, X509Certificate certificate,
                 X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) {
                return true;
            }


            // If there are errors in the certificate chain ...
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0) {
                if (chainPolicy != null) {
                    chain.ChainPolicy = chainPolicy;
                    if (!chain.Build((X509Certificate2)certificate)) {
                        throw new AikServerAuthenticationException("Certificate not trusted.");
                    }
                }
            }

            return false;
        }

        #region Signature Verification

        private void verifyXmlSignature(XmlElement xmlElement)
        {
            // Create a new SignedXml object and pass it 
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlElement);

            // Find the "Signature" node and create a new 
            // XmlNodeList object.
            XmlNodeList signatureNodeList = xmlElement.GetElementsByTagName("Signature", namespaceURI.digital_signature);

            // Throw an exception if no signature was found. 
            if (signatureNodeList.Count < 1) {
                throw new AikServerAuthenticationException("Verification failed: No Signature was found in the message.");
            }

            // This method only supports one signature for 
            // the entire XML document.  Throw an exception  
            // if more than one signature was found. 
            if (signatureNodeList.Count > 1) {
                throw new AikServerAuthenticationException("Verification failed: More that one signature was found for the message.");
            }

            // Load the signature node.
            signedXml.LoadXml((XmlElement)signatureNodeList[0]);

            X509Certificate2 signingCertificate;

            // Identify whether the whole certificate or only the subject name is included in the response
            XmlNodeList certNodeList = xmlElement.GetElementsByTagName("X509Certificate", namespaceURI.digital_signature);
            if (certNodeList.Count > 0) {
                signingCertificate = getCertificateFromResponse(certNodeList[0]);
                if (!signedXml.CheckSignature(signingCertificate, true)) {
                    throw new AikServerAuthenticationException("Invalid signature.");
                }
            }
            else {
                signingCertificate = getCertificateFromStore(xmlElement);
                if (!signedXml.CheckSignature(signingCertificate, true)) {
                    throw new AikServerAuthenticationException("Invalid signature.");
                }
            }

            if (chainPolicy == null) {
                if (!signingCertificate.Verify()) {
                    throw new AikServerAuthenticationException("Certificate not trusted.");
                }
            }
            else {
                X509Chain ch = new X509Chain();
                ch.ChainPolicy = chainPolicy;
                if (!ch.Build(signingCertificate)) {
                    throw new AikServerAuthenticationException("Certificate not trusted.");
                }
            }
        }

        private static X509Certificate2 getCertificateFromResponse(XmlNode certNode)
        {
            string str = certNode.InnerText;
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            X509Certificate2 cert = new X509Certificate2(bytes);
            return cert;
        }

        private X509Certificate2 getCertificateFromStore(XmlElement xmlElement)
        {
            // Obtain the subject name of the certificate from the signed response.
            XmlNodeList nodeList = xmlElement.GetElementsByTagName("X509SubjectName", namespaceURI.digital_signature);
            if (nodeList.Count == 0) {
                throw new AikServerAuthenticationException("Subject name of the signing certificate not found.");
            }
            DistinguishedName subjectDN = new DistinguishedName(nodeList[0].InnerText);
            X500DistinguishedName convertedSubjectDn = subjectDN.x500dn();
            string sdn = convertedSubjectDn.Name;

            //Obtain and return the certificate with the identified subject name from the trust store.
            pcTrustStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection collection = (X509Certificate2Collection)pcTrustStore.Certificates;
            X509Certificate2Collection fcollection =
                (X509Certificate2Collection)collection.Find(X509FindType.FindBySubjectDistinguishedName, sdn, false);

            if (fcollection.Count == 0) {
                throw new AikServerAuthenticationException("Certificate with the identified subject name does not exist in the certificate store.");
            }
            else if (fcollection.Count == 1) {
                return fcollection[0];
            }
            else if (fcollection.Count > 1) {
                throw new AikServerAuthenticationException("More than one certificate with the identified subject name in the certificate store.");
            }

            throw new AikServerAuthenticationException("Failed to obtain certificate with the identified subject name from the certificate store.");
        }

        #endregion

        /// <summary>
        /// Converts the DateTime format to a string to be set as an XML global time value.
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>string</returns>
        public static string formatGlobalTimeForXml(DateTime dt)
        {
            return formatLocalTimeForXml(dt) + "Z";
        }

        /// <summary>
        /// Convert the DateTime format to a string to be set as an XML local time value.
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>string</returns>
        public static string formatLocalTimeForXml(DateTime dt)
        {
            return dt.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        internal static void writeXmlElementToFile(XmlNode xmlNode, string fileName)
        {
            string strXmlText = "";
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
            XmlTextWriter xtw = null;
            try {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                xmlNode.WriteTo(xtw);
            }
            finally {
                if (xtw != null) {
                    xtw.Close();
                }
            }
            strXmlText = sb.ToString();
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
            file.WriteLine(strXmlText);
            file.Close();
        }
    }
}
