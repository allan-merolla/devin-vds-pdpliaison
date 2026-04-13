using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using PdpLiaison.Exceptions;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Xml;

namespace PdpLiaison
{
    /// <summary>
    /// This type of PDP connector should be used when the 
    /// client is designed to connect to the PDP using HTTPS.
    /// </summary>
    public class ClientSslConnector: PdpConnector
    {
        /// <summary>
        /// The certificate that the AIK uses to establish an SSL connection to the PDP.
        /// </summary>
        public X509Certificate2 clientCertificate
        {
            get { return pcClientSslCertificate; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ClientSslConnector() :
            base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdpUrl">The URL of the PDP.</param>
        /// <param name="clientCert">The certificate that the AIK uses to establish an SSL connection to the PDP.</param>
        /// <param name="trustStore">The AIK searches this certificate store to find the certificate that the server has used to 
        /// sign the responses when only the subject name of the signing certificate is included in the signature.</param>
        /// <param name="verifySignature">The signatures on the PDP responses will be checked if this is set to true.</param>
        /// <param name="communicationType">The type of communication with the PDP.</param>
        public ClientSslConnector(Uri pdpUrl, X509Certificate2 clientCert,
            X509Store trustStore, bool verifySignature, CommunicationType communicationType)
            : base(pdpUrl, communicationType, trustStore, verifySignature)
        {
            if (!pdpUrl.AbsoluteUri.ToLower().StartsWith("https")) {
                throw new AikSecurityException("The URL of the server must begin with 'https' when using ClientSslConnector");
            }

            // Client Keypair must present in ClientSslConnector
            if (clientCert == null)
            {
                throw new AikSecurityException("Client certificate cant be null.");
            }
            else
            {
                this.pcClientSslCertificate = clientCert;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdpUrl">The URL of the PDP.</param>
        /// <param name="clientCert">The certificate that the AIK use to establish an SSL connection to the PDP</param>
        /// <param name="trustStore">The AIK searches this certificate store to find the certificate that the server has used to 
        /// sign the responses when only the subject name of the signing certificate is included in the signature.</param>
        /// <param name="verifySignature">The signatures on the PDP responses will be checked if this is set to true.</param>
        /// <param name="communicationType">The type of communication with the PDP.</param>
        /// <param name="chainPolicy">The AIK, by default, builds a simple chain for the certificates and applies the 
        /// base policy to that chain. But if this parameter is set, then it will be applied to the chain.</param>
        public ClientSslConnector(Uri pdpUrl, X509Certificate2 clientCert,
            X509Store trustStore, bool verifySignature, CommunicationType communicationType, X509ChainPolicy chainPolicy)
            : base(pdpUrl, communicationType, trustStore, verifySignature, chainPolicy)
        {
            if (!pdpUrl.AbsoluteUri.ToLower().StartsWith("https")) {
                throw new AikSecurityException("The URL of the server must begin with 'https' when using ClientSslConnector");
            }

            // Client Keypair must present in ClientSslConnector
            if (clientCert == null)
            {
                throw new AikSecurityException("Client certificate cant be null.");
            }
            else
            {
                this.pcClientSslCertificate = clientCert;
            }
        }

        /// <summary>
        /// Sends the AuthorizationRequest to the PDP, receives and processes the response 
        /// from the PDP, and builds and returns an AuthorizationResponse object.
        /// </summary>
        /// <param name="request">AuthorizationRequest object to be evaluated</param>
        /// <returns>AuthorizationResponse</returns>
        public override AuthorizationResponse evaluate(AuthorizationRequest request)
        {
            AuthorizationResponse response = null;

            switch (communicationType) {
                case CommunicationType.XML_REST:
                    XmlDocument samlReq = addSamlWrapper(request);
                    XmlDocument samlRes = sendXmlRequest(samlReq, true);
                    try {
                        //Result
                        XmlNodeList nodeList = samlRes.GetElementsByTagName("Result", namespaceURI.xacml_schema);
                        if (nodeList.Count > 0) {
                            XmlNode resultNode = nodeList[0];
                            response = new AuthorizationResponse(resultNode);
                        }
                    }
                    catch (Exception ex) {
                        throw new AikException("Error in getting the Results element", ex);
                    }
                    break;
                case CommunicationType.JSON_REST:
                    response = new AuthorizationResponse(SingleResponse(httpPostJson(request.jsonRepresentation(true))));
                    break;
                default:
                    samlReq = addSamlWrapper(request);
                    XmlDocument soapReq = addSoapWrapper(samlReq);
                    XmlDocument soapRes = sendXmlRequest(soapReq, true);
                    try {
                        //Result
                        XmlNodeList nodeList = soapRes.GetElementsByTagName("Result", namespaceURI.xacml_schema);
                        if (nodeList.Count > 0) {
                            XmlNode resultNode = nodeList[0];
                            response = new AuthorizationResponse(resultNode);
                        }
                    }
                    catch (Exception ex) {
                        throw new AikException("Error in getting the Results element", ex);
                    }
                    break;
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
            MultiResponse multiResponse = null;

            switch (communicationType) {
                case CommunicationType.XML_REST:
                    XmlDocument samlReq = addSamlWrapper(multiRequest);
                    XmlDocument samlRes = sendXmlRequest(samlReq, true);
                    try {
                        XmlNodeList nodeList = samlRes.GetElementsByTagName("Response", namespaceURI.xacml_schema);
                        if (nodeList.Count > 0) {
                            XmlNode responseNode = nodeList[0];
                            multiResponse = new MultiResponse(responseNode);
                        }
                    }
                    catch (Exception ex) {
                        throw new AikException("Error in getting the Results element", ex);
                    }
                    break;
                case CommunicationType.JSON_REST:
                    multiResponse = new MultiResponse(httpPostJson(multiRequest.jsonRepresentation(true)));
                    break;
                default:
                    samlReq = addSamlWrapper(multiRequest);
                    XmlDocument soapReq = addSoapWrapper(samlReq);
                    XmlDocument soapRes = sendXmlRequest(soapReq, true);
                    try {
                        XmlNodeList nodeList = soapRes.GetElementsByTagName("Response", namespaceURI.xacml_schema);
                        if (nodeList.Count > 0) {
                            XmlNode responseNode = nodeList[0];
                            multiResponse = new MultiResponse(responseNode);
                        }
                    }
                    catch (Exception ex) {
                        throw new AikException("Error in getting the Results element", ex);
                    }
                    break;
            }

            foreach (AuthorizationResponse resp in multiResponse.responses) {
                resp.setDenyBiasedResult(recognisedObligations);
            }

            return multiResponse;
        }
    }
}
