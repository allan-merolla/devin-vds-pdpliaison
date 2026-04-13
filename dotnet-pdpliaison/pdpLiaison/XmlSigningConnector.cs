using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;
using PdpLiaison.Exceptions;

namespace PdpLiaison
{
    /// <summary>
    /// This type of PDP connector should be used when the 
    /// client is designed to sign the requests before sending them to the PDP.
    /// </summary>
    public class XmlSigningConnector: PdpConnector
    {
        private X509Certificate2 pcSigningCert;
        private CertificateInclusion pcCertificateInclusion;

        /// <summary>
        /// Certificate used for signing requests.
        /// </summary>
        public X509Certificate2 signingCert
        {
            get { return pcSigningCert; }
        }

        /// <summary>
        /// Determines whether only the signing certificate or all of the certificates 
        /// in the certificate chain will be included in the signature.
        /// </summary>
        public CertificateInclusion certificateInclusion
        {
            get { return pcCertificateInclusion; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public XmlSigningConnector() :
            base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdpUrl">The URL of the PDP.</param>
        /// <param name="communicationType">The type of communication with the PDP. JSON_REST is not a valid option for this class.</param>
        /// <param name="signingCert">The certificate to be used for signing requests.</param>
        /// <param name="certificateInclusion">Determines whether the signing certificate only or 
        /// all of the certificates in the certificate chain should be included in the signature.</param>
        /// <param name="trustStore">The AIK searches this certificate store to find the certificate that the server has used to 
        /// sign the responses when only the subject name of the signing certificate is included in the signature.</param>
        /// <param name="verifySignature">The signatures on the PDP responses will be checked if this is set to true.</param>
        public XmlSigningConnector(Uri pdpUrl, CommunicationType communicationType, X509Certificate2 signingCert,
            CertificateInclusion certificateInclusion, X509Store trustStore, bool verifySignature)
            : base(pdpUrl, communicationType, trustStore, verifySignature)
        {
            if (communicationType == CommunicationType.JSON_REST) {
                throw new AikSecurityException("CommunicationType.JSON_REST is not a valid communication type for XmlSigningConnector.");
            }
            this.pcSigningCert = signingCert;
            this.pcCertificateInclusion = certificateInclusion;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdpUrl">The URL of the PDP.</param>
        /// <param name="communicationType">The type of communication with the PDP. JSON_REST is not a valid option for this class.</param>
        /// <param name="signingCert">The certificate to be used for signing requests</param>
        /// <param name="certificateInclusion">Determines whether the signing certificate only or 
        /// all of the certificates in the certificate chain should be included in the signature.</param>
        /// <param name="trustStore">The AIK searches this certificate store to find the certificate that the server has used to 
        /// sign the responses when only the subject name of the signing certificate is included in the signature.</param>
        /// <param name="verifySignature">The signatures on the PDP responses will be checked if this is set to true.</param>
        /// <param name="chainPolicy">The AIK, by default, builds a simple chain for the certificates and applies the 
        /// base policy to that chain. But if this parameter is set, then it will be applied to the chain.</param>
        public XmlSigningConnector(Uri pdpUrl, CommunicationType communicationType, X509Certificate2 signingCert,
            CertificateInclusion certificateInclusion, X509Store trustStore, bool verifySignature, 
            X509ChainPolicy chainPolicy)
            : base(pdpUrl, communicationType, trustStore, verifySignature, chainPolicy)
        {
            if (communicationType == CommunicationType.JSON_REST) {
                throw new AikSecurityException("CommunicationType.JSON_REST is not a valid communication type for XmlSigningConnector.");
            }
            this.pcSigningCert = signingCert;
            this.pcCertificateInclusion = certificateInclusion;
        }

        /// <summary>
        /// Sends the AuthorizationRequest to the PDP, receives and processes the response 
        /// from the PDP, and builds and returns an AuthorizationResponse object.
        /// </summary>
        /// <param name="request">AuthorizationRequest object to be evaluated.</param>
        /// <returns>AuthorizationResponse</returns>
        public override AuthorizationResponse evaluate(AuthorizationRequest request)
        {
            // Add SAML wrapper
            XmlDocument samlReq = addSamlWrapper(request);
        
            // Sign the SAML node
            samlReq.PreserveWhitespace = true;
            XmlNodeList nodeList = samlReq.GetElementsByTagName("XACMLAuthzDecisionQuery", namespaceURI.xacml_saml_schema);
            if (nodeList.Count < 1) {
                throw new AikException("XACMLAuthzDecisionQuery not found.");
            }
            XmlNode queryNode = nodeList[0];
            XmlDocument signedRequest = signXmlRequest(samlReq, queryNode, true);

            // Add SOAP wrapper
            XmlDocument xmlMsg = signedRequest;
            if (communicationType == CommunicationType.XML_SOAP) {
                xmlMsg = addSoapWrapper(signedRequest);
            }
            
            // Send message
            XmlDocument xmlResponse = sendXmlRequest(xmlMsg, false);

            if (samlStatus == "urn:oasis:names:tc:SAML:2.0:status:AuthnFailed") {
                throw new AikClientAuthenticationException("Authentication failed.");
            }

            // Get result element
            AuthorizationResponse response = null;
            try {
                nodeList = xmlResponse.GetElementsByTagName("Result", namespaceURI.xacml_schema);
                if (nodeList.Count > 0) {
                    XmlNode resultNode = nodeList[0];
                    response = new AuthorizationResponse(resultNode);
                }
            }
            catch (Exception ex) {
                throw new AikException("Error in getting the Results element", ex);
            }

            response.setDenyBiasedResult(recognisedObligations);

            return response;
        }

        /// <summary>
        /// Sends the MultiRequest to the PDP, receives and processes the response 
        /// from the PDP, and builds and returns a MultiResponse object.
        /// </summary>
        /// <param name="multiRequest">MultiRequest object to be evaluated</param>
        /// <returns>MultiResponse</returns>
        public override MultiResponse evaluate(MultiRequest multiRequest)
        {
            //Add SAML Wrapper
            XmlDocument samlReq = addSamlWrapper(multiRequest);
     
            //Sign the SMAL node
            samlReq.PreserveWhitespace = true;
            XmlNodeList nodeList = samlReq.GetElementsByTagName("XACMLAuthzDecisionQuery", namespaceURI.xacml_saml_schema);
            if (nodeList.Count < 1) {
                throw new AikException("XACMLAuthzDecisionQuery not found.");
            }
            XmlNode queryNode = nodeList[0];
            XmlDocument signedRequest = signXmlRequest(samlReq, queryNode, true);
            
            //Add SOAP wrapper
            XmlDocument xmlMsg = signedRequest;
            if (communicationType == CommunicationType.XML_SOAP) {
                xmlMsg = addSoapWrapper(signedRequest);
            }
            //Send SOAP message
            MultiResponse multiResponse = null;
            XmlDocument xmlRes = sendXmlRequest(xmlMsg, false);

            if (samlStatus == "urn:oasis:names:tc:SAML:2.0:status:AuthnFailed") {
                throw new AikClientAuthenticationException("Authentication failed.");
            }

            try {
                nodeList = xmlRes.GetElementsByTagName("Response", namespaceURI.xacml_schema);
                if (nodeList.Count > 0) {
                    XmlNode responseNode = nodeList[0];
                    multiResponse = new MultiResponse(responseNode);
                }
            }
            catch (Exception ex) {
                throw new AikException("Error in getting the Results element", ex);
            }

            foreach (AuthorizationResponse resp in multiResponse.responses) {
                resp.setDenyBiasedResult(recognisedObligations);
            }

            return multiResponse;
        }

        private XmlDocument signXmlRequest(XmlDocument unsignedDoc,
            XmlNode unsignedQueryNode, bool exclusive)
        {
            if (signingCert == null) {
                throw new AikException("Signing certificate is not available.");
            }

            // Create a new XML document.
            XmlDocument signedDoc = new XmlDocument();
            signedDoc.PreserveWhitespace = true;
            signedDoc = (XmlDocument)unsignedDoc.CloneNode(true);

            XmlNodeList nodeList = signedDoc.GetElementsByTagName(unsignedQueryNode.LocalName, unsignedQueryNode.NamespaceURI);

            if (nodeList.Count < 1) {
                throw new AikException("XACMLAuthzDecisionQuery not found.");
            }

            XmlNode toBeSignedQueryNode = nodeList[0];

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml((XmlElement)toBeSignedQueryNode);

            // Add the key to the SignedXml document. 
            if (signingCert.PrivateKey == null) {
                throw new AikException("Private key of the  signing certificate is not available.");
            }
            signedXml.SigningKey = signingCert.PrivateKey;

            // Specify a canonicalization method.
            if (exclusive == true) {
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            }
            else {
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
            }
                                                                   
            // Create a reference to be signed.
            string id = toBeSignedQueryNode.Attributes["ID"].Value;
            Reference reference = new Reference();
            reference.Uri = "#" + id;

            // Add an enveloped transformation to the reference.
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            // Add an enveloped transformation to the reference.
            if (exclusive == true) {
                reference.AddTransform(new XmlDsigExcC14NTransform());
            }
            else {
                reference.AddTransform(new XmlDsigC14NTransform());
            }

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Add an RSAKeyValue KeyInfo 
            KeyInfo keyInfo = new KeyInfo();
            if (pcCertificateInclusion == CertificateInclusion.certificateChain) {
                // Adding the whole certificate chain
                addCertificateChain(keyInfo);
            }
            else {
                // Adding the signing certificate only
                keyInfo.AddClause(new KeyInfoX509Data(signingCert));
            }
            signedXml.KeyInfo = keyInfo;

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save 
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            nodeList = signedDoc.GetElementsByTagName("Request", namespaceURI.xacml_schema);
            if (nodeList.Count > 0) {
                XmlNode reqNode = nodeList[0];
                toBeSignedQueryNode.InsertBefore(signedDoc.ImportNode(xmlDigitalSignature, true), reqNode);
            }
            return signedDoc;
        }

        private void addCertificateChain(KeyInfo keyInfo)
        {
            X509Chain ch = new X509Chain();
            ch.Build(signingCert);
            foreach (X509ChainElement element in ch.ChainElements) {
                if (element.Certificate.Issuer == element.Certificate.Subject) {
                    break;
                }
                keyInfo.AddClause(new KeyInfoX509Data(element.Certificate));
            }
        }
    }
}
